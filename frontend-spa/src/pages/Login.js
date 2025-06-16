import React, { useState } from 'react';
import './Login.css';

function Login() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleLogin = async () => {
    setLoading(true);
    setError('');

    try {
      const res = await fetch('http://localhost:5091/api/v1/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password }),
      });

      if (!res.ok) throw new Error('Invalid email or password');

      const data = await res.json();
      localStorage.setItem('token', data.token);
      window.location.href = '/movies';
    } catch (err) {
      setError(err.message || 'Login failed');
    } finally {
      setLoading(false);
    }
  };

  const handleSSOLogin = () => {
    window.location.href =
      'https://sudhirthakur.eu.auth0.com/authorize?client_id=Xob9i97mhQwS9SaLI3oNnelUbtgeFBZP&response_type=token&redirect_uri=http://localhost:3000/callback&scope=openid profile email&audience=https://moviesystem/api';
  };

  return (
    <div className="login-container">
      <div className="login-box">
        <h2>Login</h2>

        <input
          type="email"
          placeholder="Email"
          value={email}
          onChange={e => setEmail(e.target.value)}
        />

        <input
          type="password"
          placeholder="Password"
          value={password}
          onChange={e => setPassword(e.target.value)}
        />

        {error && <p className="error">{error}</p>}

        <button onClick={handleLogin} disabled={loading}>
          {loading ? 'Logging in...' : 'Login'}
        </button>

        <div className="divider">OR</div>

        <button className="sso-button" onClick={handleSSOLogin}>
          Login with SSO
        </button>
      </div>
    </div>
  );
}

export default Login;
