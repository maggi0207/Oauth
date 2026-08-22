Add-Type -AssemblyName System.Windows.Forms

Write-Host "Keep Awake is running. Press Ctrl+C to stop."

while ($true) {
    [System.Windows.Forms.SendKeys]::SendWait("{SCROLLLOCK}")
    Start-Sleep -Milliseconds 100
    [System.Windows.Forms.SendKeys]::SendWait("{SCROLLLOCK}")

    Start-Sleep -Seconds 60
}
