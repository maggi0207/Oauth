// src/components/LoadingSpinner.js
import React from 'react';
import CircularProgress from '@mui/material/CircularProgress';
import Box from '@mui/material/Box';

const LoadingSpinner = () => {
  return (
    <Box
      display="flex"
      justifyContent="center"
      alignItems="center"
      minHeight="60vh"
    >
      <CircularProgress color="primary" size={60} thickness={4} />
    </Box>
  );
};

export default LoadingSpinner;
