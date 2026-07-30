I've been investigating the UI lag issue where typing in an input and immediately clicking another control (button, Date Picker, Cancel, etc.) causes the UI to freeze for 4–5 seconds before updating.

To isolate the issue, I removed all business logic and reduced the page to a simple input and button. The issue still occurred. I then created a fresh Vite + React application with only an input and button, and the same behavior was reproduced. I also created a fresh Angular application with the same result, which rules out our application code and the React framework.

I tested the applications in different environments and found that they work normally in the VS Code built-in browser, but the lag consistently occurs in Chrome and Edge.

Based on these findings, the issue appears to be related to the browser or the corporate environment rather than the application itself. Possible causes include browser policies, extensions, Zscaler, Citrix Browser Content Redirection (BCR), or other enterprise security software.
