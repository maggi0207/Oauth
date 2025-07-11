import { useEffect, useState, useRef } from 'react';
import { useAuth0 } from '@auth0/auth0-react';

export const useTokenExpiryDialog = () => {
  const { logout, getAccessTokenSilently, user, getIdTokenClaims, isAuthenticated } = useAuth0();
console.log('[Token user]', user);
  const [showDialog, setShowDialog] = useState(false);
  const [secondsRemaining, setSecondsRemaining] = useState(60);

  const countdownRef = useRef(null);
  const warningTimerRef = useRef(null);
 
  useEffect(() => {
    if (!isAuthenticated) {
      console.log('[Auth] Not authenticated — skipping timers');
      return;
    }

    const setupTokenTimers = async () => {
      try {
        const claims = await getIdTokenClaims();
        console.log('[Token Claims]', claims);

        if (!claims?.exp) {
          console.warn('[Token] Missing exp field');
          return;
        }

        const expiry = claims.exp * 1000;
        const now = Date.now();

        const timeUntilWarning = expiry - now - 60 * 1000;

        console.log('[Timing Info]', {
          now: new Date(now).toISOString(),
          expiry: new Date(expiry).toISOString(),
          timeUntilWarning,
        });

        if (expiry <= now) {
          console.warn('[Token] Already expired — logging out');
          logout({ returnTo: window.location.origin });
          setTimeout(() => window.location.replace(window.location.origin), 100);
          return;
        }

        if (timeUntilWarning > 0) {
          warningTimerRef.current = setTimeout(() => {
            console.log('[Dialog] Showing warning');
            setShowDialog(true);

            let countdown = 60;
            setSecondsRemaining(countdown);

            countdownRef.current = setInterval(() => {
              countdown -= 1;
              setSecondsRemaining(countdown);
              if (countdown <= 0) {
                clearInterval(countdownRef.current);
                countdownRef.current = null;
                console.log('[Auto Logout] No response from user');
                logout({ returnTo: window.location.origin });
                setTimeout(() => window.location.replace(window.location.origin), 100);
              }
            }, 1000);
          }, timeUntilWarning);
        }
      } catch (err) {
        console.error('[Setup Error]', err);
      }
    };

    setupTokenTimers();

    return () => {
      if (warningTimerRef.current) clearTimeout(warningTimerRef.current);
      if (countdownRef.current) clearInterval(countdownRef.current);
      console.log('[Cleanup] Timers cleared');
    };
  }, [isAuthenticated, getAccessTokenSilently, getIdTokenClaims, logout]);

  const stayLoggedIn = async () => {
    console.log('[User Action] Stay Logged In clicked');
    try {
      await getAccessTokenSilently({ ignoreCache: true });
      setShowDialog(false);
      if (countdownRef.current) {
        clearInterval(countdownRef.current);
        countdownRef.current = null;
      }
    } catch {
      console.error('[User Action] Failed to refresh — logging out');
      logout({ returnTo: window.location.origin });
      setTimeout(() => window.location.replace(window.location.origin), 100);
    }
  };

  const logoutUser = () => {
    console.log('[User Action] Manual logout clicked');
    if (countdownRef.current) {
      clearInterval(countdownRef.current);
      countdownRef.current = null;
    }
    logout({ returnTo: window.location.origin });
    setTimeout(() => window.location.replace(window.location.origin), 100);
  };

  return { showDialog, secondsRemaining, stayLoggedIn, logoutUser };
};
