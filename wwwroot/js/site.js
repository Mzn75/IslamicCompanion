document.addEventListener("DOMContentLoaded", function () {

    if (window.location.search.includes("timeZone")) {
        return;
    }

    const locationLoader = document.getElementById("location-loader");

    if (locationLoader) {
        // Grab their exact timezone name (e.g., "Africa/Cairo" or "Europe/London")
        // This works perfectly even if they deny GPS location!
        const userTimeZone = Intl.DateTimeFormat().resolvedOptions().timeZone;

        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(
                function (position) {
                    // Success: Grab coordinates and send them with the TimeZone string
                    const lat = position.coords.latitude;
                    const lng = position.coords.longitude;

                    window.location.href = `/Home/Index?lat=${lat}&lng=${lng}&timeZone=${userTimeZone}`;
                },
                function (error) {
                    console.warn("Location denied or failed: " + error.message);
                    showLocationError();
                },
                { timeout: 10000 }
            );
        } else {
            // Browser doesn't support GPS
            console.warn("Browser does not support geolocation.");
            showLocationError();
        }
    }

    function showLocationError() {
        // Replace the loading screen with a nice error card
        document.body.innerHTML = `
        <div style="display: flex; align-items: center; justify-content: center; height: 100vh; background-color: #f3f4f6; margin: 0;">
            <div style=" width:90%; max-width: 400px; padding: 2.5rem; background: white; border-radius: 12px; box-shadow: 0 4px 6px rgba(0,0,0,0.1); text-align: center;">
                <div style="font-size: 3rem; margin-bottom: 1rem;">📍</div>
                <h2 style="color: #ff0000; margin-bottom: 1rem; border-bottom: 2px solid #ff0000; padding-bottom: 0.5rem;">
                    Location Needed
                </h2>
                <p style="color: #374151; font-size: 1.1rem; line-height: 1.5; margin-bottom: 1.5rem;">
                    We couldn't get your location. We need it to calculate your exact prayer times. 
                    <br><br>
                    Please click the lock icon in your address bar, <strong>Allow Location</strong>, and try again.
                </p>
                <button onclick="window.location.reload()" 
                        style="padding: 10px 20px; background-color: #ff0000; color: white; border: none; border-radius: 6px; font-size: 1.1rem; font-weight: bold; cursor: pointer; transition: 0.2s;">
                    Refresh Page
                </button>
            </div>
        </div>
    `;
    }

});

/// Sidebar Toggle Logic
const sidebarToggle = document.getElementById("sidebarToggle");
const sidebar = document.getElementById("sidebar");
const overlay = document.getElementById("sidebarOverlay");

if (sidebarToggle && sidebar && overlay) {
    function toggleSidebar() {
        sidebar.classList.toggle("open");
        overlay.classList.toggle("show");

        // NEW: Locks/unlocks the background scrolling
        document.body.classList.toggle("no-scroll");
    }

    // Open menu when clicking the hamburger button
    sidebarToggle.addEventListener("click", toggleSidebar);

    // Close menu when clicking the dark background overlay
    overlay.addEventListener("click", toggleSidebar);
}