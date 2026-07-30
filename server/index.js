I've been investigating the UI lag issue where, after typing in an input field and immediately clicking another control (such as a button, Date Picker, Cancel, or Close), the click is registered but the UI does not update for about 4–5 seconds. Once the delay is over, the pending UI updates appear and the application becomes responsive again.

To isolate the problem, I first removed all the business logic related to the Roll Day field and replaced it with a simple useState input. I then removed the entire AveragingScheduleTab component and even reduced the page to just a basic input and button. The issue still occurred.

To verify whether it was application-specific, I created a brand new Vite + React application containing only an input and a button with a counter. The same lag was reproduced. I then created a fresh Angular application with the same simple functionality, and the issue was still present. This suggests that the problem is not specific to our application or to React.

I also removed React Strict Mode, tested outside the OneDrive folder, and confirmed there was no expensive rendering or business logic executing during the delay. None of these changes made any difference.

The biggest finding came from testing in different browsers. When I run the application inside the VS Code built-in browser, everything works normally without any lag. However, when I open the exact same application in Google Chrome or Microsoft Edge, the issue is consistently reproduced.

Based on these findings, it appears that the issue is not related to our application code but is more likely caused by the browser environment on the corporate machine. Possible causes could include browser policies, extensions, Zscaler, Citrix Browser Content Redirection (BCR), or other enterprise security software affecting Chrome and Edge.
