import React from 'react';
import Dialog from '@mui/material/Dialog';
import DialogTitle from '@mui/material/DialogTitle';
import DialogContent from '@mui/material/DialogContent';
import DialogActions from '@mui/material/DialogActions';
import Button from '@mui/material/Button';
import Typography from '@mui/material/Typography';
import WarningAmberRoundedIcon from '@mui/icons-material/WarningAmberRounded';
import { useTokenExpiryDialog } from '../hooks/useTokenExpiryDialog';

/**
 * SessionExpiryDialog - Warns the user before session expires, with improved styling and countdown.
 */
const SessionExpiryDialog = () => {
  const { showDialog, stayLoggedIn, logoutUser, secondsRemaining } = useTokenExpiryDialog();

  return (
    <Dialog
      open={showDialog}
      onClose={() => {}}
      disableEscapeKeyDown
      PaperProps={{
        sx: {
          borderRadius: 3,
          padding: 2,
          backgroundColor: '#fff3e0',
          boxShadow: 6,
        },
      }}
    >
      <DialogTitle sx={{ display: 'flex', alignItems: 'center', gap: 1, color: '#ff6f00' }}>
        <WarningAmberRoundedIcon fontSize="medium" />
        Session Expiring Soon
      </DialogTitle>

      <DialogContent>
        <Typography variant="body1" sx={{ mb: 1 }}>
          Your session will expire in <strong>{secondsRemaining}</strong> seconds.
        </Typography>
        <Typography variant="body2" sx={{ color: 'text.secondary' }}>
          To stay signed in, choose “Stay Logged In.” Otherwise, you will be automatically logged out.
        </Typography>
      </DialogContent>

      <DialogActions>
        <Button onClick={logoutUser} variant="outlined" color="error">
          Logout
        </Button>
        <Button onClick={stayLoggedIn} variant="contained" color="warning" autoFocus>
          Stay Logged In
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default SessionExpiryDialog;
