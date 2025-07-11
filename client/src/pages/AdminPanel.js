import React from 'react';

const AdminPanel = () => {
  return (
    <div style={styles.container}>
      <h1>🛠 Admin Dashboard</h1>
      <p>Welcome to the Admin section. Manage everything from user access to system logs.</p>

      <ul>
        <li>Manage Roles & Permissions</li>
        <li>System Audit Logs</li>
        <li>Backup & Recovery</li>
        <li>Configuration Settings</li>
      </ul>
    </div>
  );
};

const styles = {
  container: {
    padding: 24,
    fontFamily: 'Arial, sans-serif',
  },
};

export default AdminPanel;
