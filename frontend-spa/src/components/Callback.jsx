import { useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";

function Callback() {
  const navigate = useNavigate();
  const location = useLocation();

  useEffect(() => {
    // Only run if we are really on /callback
    if (!location.pathname.includes("/callback")) return;

    const { hash } = window.location;
    if (hash.includes("access_token")) {
      const params = new URLSearchParams(hash.substring(1));
      const accessToken = params.get("access_token");

      if (accessToken) {
        localStorage.setItem("token", accessToken);
        window.history.replaceState(null, "", "/callback");
        navigate("/movies");
      } else {
        console.error("Access token not found in hash.");
      }
    } else {
      console.warn("Access token not found in callback URL.");
    }
  }, [location, navigate]);

  return <div>Processing login...</div>;
}

export default Callback;
