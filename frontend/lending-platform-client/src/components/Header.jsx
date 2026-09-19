import React from 'react';
import { Activity, ServerOff } from 'lucide-react';

const Header = ({ isApiOnline }) => (
  <header className="header">
    <div className="header-inner">
      <div className="brand">
        <h1>LendCore</h1>
        <p>Intelligent lending decision platform</p>
      </div>

      <div
        className={`status-pill ${isApiOnline ? 'status-pill--online' : 'status-pill--offline'}`}
        role="status"
        aria-live="polite"
        aria-label={isApiOnline ? 'Decision engine online' : 'Unable to connect to decision engine'}
      >
        {isApiOnline ? (
          <>
            <Activity size={13} aria-hidden="true" />
            Decision engine online
          </>
        ) : (
          <>
            <ServerOff size={13} aria-hidden="true" />
            Engine offline
          </>
        )}
      </div>
    </div>
  </header>
);

export default Header;
