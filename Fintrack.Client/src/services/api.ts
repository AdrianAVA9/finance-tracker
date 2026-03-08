import axios from 'axios';

// Create a configured axios instance
// The Vite proxy running on port 5173 points relative requests to the backend.
const api = axios.create({
  baseURL: '/', // Use the same domain to allow cookies to work properly via Vite's proxy
  withCredentials: true, // This is CRITICAL for sending HttpOnly cookies!
});

export default api;
