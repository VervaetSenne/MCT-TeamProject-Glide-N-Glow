// Check if the user is logged in as admin
const isAdminLoggedIn = localStorage.getItem('AdminLoggedIn') === 'true';
var fetchdom = 'http://localhost:5165';

//Available gamemodes vars
var availableGamemodes;

//Current gamemode vars
var currentGamemodes;

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
      var fetchlink = ``;
      if (data.optionText == 'User gamemode switch') {
        fetchlink = `${fetchdom}/admin/allow-game-switch/${data.isChecked}`;
        console.log(fetchlink);
      }
      if (data.optionText == 'Gamemode force') {
        fetchlink = `${fetchdom}/admin/force-gamemode/${data.isChecked}`;
        console.log(fetchlink);
      }
      if (data.optionText == 'Lighting') {
        fetchlink = `${fetchdom}/admin/lightning/${data.isChecked}`;
        console.log(fetchlink);
      }
      if (data.optionText == 'Start calibration') {
        fetchlink = '';
      }

      fetch(fetchlink, {
        method: 'POST', // or 'PUT', 'DELETE', etc., depending on your API
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
      })
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
  const checkboxStates = {}; // Object to store checkbox states

  //fetch data
  fetch(`${fetchdom}/gamemode/all`)
    .then((response) => {
      if (!response.ok) {
        throw new Error(
          `Failed to fetch gamemodes. Status: ${response.status}`
        );
      }
      return response.json();
    })
    .then((gamemodes) => {
      // Assuming your API response is an array of gamemodes
      console.log(gamemodes);
      let html = '';
      for (const gamemode of gamemodes) {
        html += `<div class="grid-option">
                  <span class="option-text-availablegamemodes">${gamemode.name}</span>
                  <label class="checkbox-container">
                    <input type="checkbox" id="${gamemode.id}" />
                    <span class="checkmark"></span>
                  </label>
                </div>
                <hr class="option-divider" />`;

        // Initialize checkbox state in the object -- get from api later
        checkboxStates[gamemode.id] = false;
      }
      availableGamemodes.innerHTML = html;

      // Attach event listener to each checkbox
      const checkboxes = document.querySelectorAll(
        '.checkbox-container input[type="checkbox"]'
      );
      var ListCheckedModes = [];

      checkboxes.forEach((checkbox) => {
        checkbox.addEventListener('change', function () {
          checkboxStates[this.id] = this.checked; // Update checkbox state
          if (this.checked) {
            ListCheckedModes.push(this.id);
          } else {
            const index = ListCheckedModes.indexOf(this.id);
            if (index !== -1) {
              ListCheckedModes.splice(index, 1);
            }
          }

          console.log(ListCheckedModes);

          // Post to the API
          fetch(`${fetchdom}/admin/available`, {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
            },
            body: JSON.stringify(ListCheckedModes),
          })
            .then((result) => {
              // Handle the API response if needed
              console.log('API Response:', result);
            })
            .catch((error) => {
              // Handle errors
              console.error('Error posting checked modes to API:', error);
            });
        });
      });
    })
    .catch((error) => {
      console.error('Error fetching gamemodes:', error);
    });
}

function checkCurrentGamemodes() {
  //fetch data
  fetch(`${fetchdom}/gamemode/all`)
    .then((response) => {
      if (!response.ok) {
        throw new Error(
          `Failed to fetch gamemodes. Status: ${response.status}`
        );
      }
      return response.json();
    })
    .then((gamemodes) => {
      // Assuming your API response is an array of gamemodes
      console.log(gamemodes);
      let html = '';
      for (const gamemode of gamemodes) {
        html += `<div class="gamemode-box">
                  <div class="row">
                    <svg
                      xmlns="http://www.w3.org/2000/svg"
                      width="40"
                      height="40"
                      viewBox="0 0 24 24"
                      fill="none"
                      stroke="currentColor"
                      stroke-width="2"
                      stroke-linecap="round"
                      stroke-linejoin="round"
                      class="lucide lucide-gamepad-2"
                    >
                      <line x1="6" x2="10" y1="11" y2="11" />
                      <line x1="8" x2="8" y1="9" y2="13" />
                      <line x1="15" x2="15.01" y1="12" y2="12" />
                      <line x1="18" x2="18.01" y1="10" y2="10" />
                      <path
                        d="M17.32 5H6.68a4 4 0 0 0-3.978 3.59c-.006.052-.01.101-.017.152C2.604 9.416 2 14.456 2 16a3 3 0 0 0 3 3c1 0 1.5-.5 2-1l1.414-1.414A2 2 0 0 1 9.828 16h4.344a2 2 0 0 1 1.414.586L17 18c.5.5 1 1 2 1a3 3 0 0 0 3-3c0-1.545-.604-6.584-.685-7.258-.007-.05-.011-.1-.017-.151A4 4 0 0 0 17.32 5z"
                      />
                    </svg>
                  </div>
                  <div class="row title">${gamemode.name}</div>
                  <div class="row description">
                    This is the description for ${gamemode.name}.
                  </div>
                </div>`;
      }
      currentGamemodes.innerHTML = html;

      // Get all gamemode-box elements
      const gamemodeBoxes = document.querySelectorAll('.gamemode-box');

      // Add click event listener to each gamemode-box
      gamemodeBoxes.forEach(function (box) {
        box.addEventListener('click', function () {
          // Toggle background color
          if (this.classList.contains('selected')) {
            this.classList.remove('selected');
            console.log(
              `Current gamemode - ${
                this.querySelector('.title').innerText
              }: false`
            );
          } else {
            // Remove 'selected' class from all gamemode-box elements
            gamemodeBoxes.forEach(function (otherBox) {
              otherBox.classList.remove('selected');
            });

            this.classList.add('selected');
            console.log(
              `Current gamemode - ${
                this.querySelector('.title').innerText
              }: true`
            );
          }
        });
      });
    })
    .catch((error) => {
      console.error('Error fetching gamemodes:', error);
    });
}

function renderCarousel() {
  console.log('Rendering carousel...');
  $(document).ready(function () {
    $('.gamemodes-carousel').slick({
      centerMode: true,
      centerPadding: '10px', // Adjust the center padding
      slidesToShow: 3,
      infinite: false,
      initialSlide: 2,
      dots: true,
      arrows: true,
      cssEase: 'ease-in-out',
      responsive: [
        {
          breakpoint: 768,
          settings: {
            arrows: false,
            centerMode: true,
            centerPadding: '40px',
            slidesToShow: 1,
          },
        },
        {
          breakpoint: 480,
          settings: {
            arrows: false,
            centerMode: true,
            centerPadding: '40px',
            slidesToShow: 1,
          },
        },
      ],
    });
  });
}

document.addEventListener('DOMContentLoaded', (event) => {
  checkUrl();
  checkAdmin();
  checkLogout();
  //checkSettings();
  //checkAvailableGamemodes();
  //checkCurrentGamemodes();
  renderCarousel();

  //Available gamemodes vars
  availableGamemodes = document.querySelector(
    '.js-available-gamemodes-container'
  );
  //Current gamemode vars
  currentGamemodes = document.querySelector('.js-current-gamemode-container');
});
