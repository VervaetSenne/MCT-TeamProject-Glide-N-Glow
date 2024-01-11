// Check if the user is logged in as admin
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
  // Show/hide the admin navigation item based on the user's login status
  const navItemAdmin = document.getElementById('nav-item-admin');
  if (isAdminLoggedIn) {
    navItemAdmin.style.display = 'block';
  } else {
    navItemAdmin.style.display = 'none';
  }
}
function checkLogout() {
  const userButton = document.getElementById('userButton');
  const navItemAdmin = document.getElementById('nav-item-admin');
  userButton.addEventListener('click', function () {
    // Set AdminLoggedIn to false and hide the admin navigation item
    localStorage.setItem('AdminLoggedIn', false);
    navItemAdmin.style.display = 'none';
    console.log('logout');
  });
}
function checkSettings() {
  const toggleCheckboxes = document.querySelectorAll(
    '.toggle input[type="checkbox"]'
  );
  toggleCheckboxes.forEach(function (checkbox) {
    checkbox.addEventListener('change', function () {
      // Get the corresponding option text
      const optionText =
        this.closest('.grid-option').querySelector('.option-text').innerText;

      // Log the state (true or false) and the option text
      console.log(`${optionText} : ${this.checked}`);

      // Prepare the data to send to the API
      const data = {
        optionText: optionText,
        isChecked: this.checked,
      };

      // Send the data to the API using fetch
      fetch('https://your-api-endpoint.com', {
        method: 'POST', // or 'PUT', 'DELETE', etc., depending on your API
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
      })
        .then((response) => response.json())
        .then((result) => {
          // Handle the API response if needed
          console.log('API Response:', result);
        })
        .catch((error) => {
          // Handle errors
          console.error('Error sending data to API:', error);
        });
    });
  });
}
function checkAvailableGamemodes() {
  const checkboxes = document.querySelectorAll('input[type="checkbox"]');

  checkboxes.forEach((checkbox) => {
    checkbox.addEventListener('change', function () {
      console.log(this.id + ': ' + this.checked);
      const data = {
        id: this.id,
        checked: this.checked,
      };
      /*
      // Assuming your API endpoint is https://example.com/api/updateCheckboxStatus
      fetch('https://example.com/api/updateCheckboxStatus', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
      })
        .then((response) => {
          if (!response.ok) {
            throw new Error('Network response was not ok');
          }
          return response.json();
        })
        .then((responseData) => {
          console.log(responseData); // Log the API response if needed
        })
        .catch((error) => {
          console.error('Error:', error);
        });*/
    });
  });
}
function checkCurrentGamemodes() {
  // Get all gamemode-box elements
  const gamemodeBoxes = document.querySelectorAll('.gamemode-box');

  // Add click event listener to each gamemode-box
  gamemodeBoxes.forEach(function (box) {
    box.addEventListener('click', function () {
      // Toggle background color
      if (this.classList.contains('selected')) {
        this.classList.remove('selected');
        console.log(
          `Current gamemode - ${this.querySelector('.title').innerText}: false`
        );
      } else {
        // Remove 'selected' class from all gamemode-box elements
        gamemodeBoxes.forEach(function (otherBox) {
          otherBox.classList.remove('selected');
        });

        this.classList.add('selected');
        console.log(
          `Current gamemode - ${this.querySelector('.title').innerText}: true`
        );
      }
    });
  });
}

document.addEventListener('DOMContentLoaded', (event) => {
  checkUrl();
  checkAdmin();
  checkLogout();
  checkSettings();
  checkAvailableGamemodes();
  checkCurrentGamemodes();
});
