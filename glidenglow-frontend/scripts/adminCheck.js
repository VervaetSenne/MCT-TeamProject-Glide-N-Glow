const isAdminLoggedIn = localStorage.getItem('AdminLoggedIn') === 'true';

function checkUrl() {
  // Check if the URL pathname is /admin.html
  if (window.location.pathname.endsWith('/admin.html')) {
    if (!isAdminLoggedIn) {
      // Ask for a password
      const enteredPassword = prompt('Enter the password for admin access:');

      // Check the password (replace 'admin123' with your actual password)
      if (enteredPassword === 'admin123') {
        // Set a local storage variable to indicate the user is logged in as admin
        localStorage.setItem('AdminLoggedIn', true);
        navItemAdmin.style.display = 'block';
      } else {
        // Redirect the user away from the admin page if the password is incorrect
        alert('Incorrect password. You will be redirected to the homepage.');
        window.location.href = '/';
      }
    }
  }
}
function checkAdmin() {
  // Show/hide the admin navigation itemS based on the user's login status
  const navItemAdmin = document.getElementById('nav-item-admin');
  const navItemLogoutAdmin = document.querySelector('.nav-item-logout-admin');
  if (isAdminLoggedIn) {
    navItemAdmin.style.display = 'block';
    navItemLogoutAdmin.style.display = 'block';
  } else {
    navItemAdmin.style.display = 'none';
    navItemLogoutAdmin.style.display = 'none';
  }
}
function checkLogout() {
  const userButton = document.getElementById('userButton');
  const navItemAdmin = document.getElementById('nav-item-admin');
  userButton.addEventListener('click', function () {
    // Set AdminLoggedIn to false and hide the admin navigation item
    localStorage.setItem('AdminLoggedIn', false);
    navItemAdmin.style.display = 'none';
    checkAdmin();
    console.log('logout');
  });
}

document.addEventListener('DOMContentLoaded', (event) => {
  checkUrl();
  checkAdmin();
  checkLogout();
});
