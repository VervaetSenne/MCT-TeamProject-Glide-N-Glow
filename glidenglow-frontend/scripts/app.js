// Check if the user is logged in as admin
const isAdminLoggedIn = localStorage.getItem('AdminLoggedIn') === 'true';
var fetchdom = 'http://localhost:5165';

//Gamemodes var
let gamemodesTable;
let AllowGamemodeSwitchLoadState;

//Buttons var
let buttonTable;

//Lightstrips var
let lightstripsTable;

//Current gamemode vars

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
    console.log('logout');
  });
}
function handleGamemodes() {
  /*
    GAMEMODES GET ALL GAMEMODES
  */
  console.log('gamemodes function');
  fetch(`${fetchdom}/gamemode/settings`)
    .then((response) => {
      if (!response.ok) {
        throw new Error(
          `Failed to fetch gamemodes. Status: ${response.status}`
        );
      }
      return response.json();
    })
    .then((gamemodes) => {
      console.log(gamemodes);
      let html = '<table class="rounded-table">';

      // Add table headers
      html += `
        <tr>
          <th>Name</th>
          <th>Set available</th>
          <th>Force</th>
        </tr>
      `;

      for (const gamemode of gamemodes.gamemodes) {
        console.log(gamemode);
        html += `
          <tr>
            <td>${gamemode.name}</td>
            <td>
              <label class="checkbox-container">
                <input type="checkbox" class="set-available-checkbox" data-id="${gamemode.id}" />
                <span class="checkmark"></span>
              </label>
            </td>
            <td>
              <button class="table-button table-button-force-gamemodes" id="${gamemode.id}">
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  width="24"
                  height="24"
                  viewBox="0 0 24 24"
                  fill="none"
                  stroke="currentColor"
                  stroke-width="2"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  class="lucide lucide-lock"
                >
                  <rect
                    width="18"
                    height="11"
                    x="3"
                    y="11"
                    rx="2"
                    ry="2"
                  />
                  <path d="M7 11V7a5 5 0 0 1 10 0v4" />
                </svg>
              </button>
            </td>
          </tr>`;
      }

      html += '</table>';
      gamemodesTable.innerHTML = html;

      /*
      SETT GAMEMODE AVAILABLE CHECKBOX
    */

      // Add event listener to all checkboxes with the class "set-available-checkbox"
      const setAvailableCheckboxes = document.querySelectorAll(
        '.set-available-checkbox'
      );
      setAvailableCheckboxes.forEach((checkbox) => {
        checkbox.addEventListener('change', () => {
          console.log(
            `Set Gamemode Available Checkbox id: ${checkbox.dataset.id} state:`,
            checkbox.checked
          );
          var setGamemodeAvailableId = checkbox.dataset.id;

          if (checkbox.checked) {
            // If checkbox is checked, POST to set gamemode available state
            fetch(`${fetchdom}/gamemode/available/${setGamemodeAvailableId}`, {
              method: 'POST',
              headers: {
                'Content-Type': 'application/json',
              },
            })
              .then((result) => {
                // Handle the API response if needed
                console.log('API Response - set gamemode available:', result);
              })
              .catch((error) => {
                // Handle errors
                console.error(
                  'Error sending data to API - set gamemode available:',
                  error
                );
              });
          } else {
            // If checkbox is unchecked, DELETE to remove gamemode available state
            fetch(`${fetchdom}/gamemode/available/${setGamemodeAvailableId}`, {
              method: 'DELETE',
              headers: {
                'Content-Type': 'application/json',
              },
            })
              .then((result) => {
                // Handle the API response if needed
                console.log(
                  'API Response - remove gamemode available:',
                  result
                );
              })
              .catch((error) => {
                // Handle errors
                console.error(
                  'Error sending data to API - remove gamemode available:',
                  error
                );
              });
          }
        });
      });

      /*
      GAMEMODES ALLOW GAMEMODE SWITCH
      */

      // Add event listener to the toggle
      const allowGamemodeToggle = document.getElementById(
        'AllowGamemodeToggle'
      );
      allowGamemodeToggle.addEventListener('change', () => {
        console.log('Allow Gamemode Switch:', allowGamemodeToggle.checked);
        var allowGamemodeSwitchState = allowGamemodeToggle.checked;
        // Post the allow gamemode switch state to api
        fetch(
          `${fetchdom}/gamemode/allow-switching/${allowGamemodeSwitchState}`,
          {
            method: 'PUT',
            headers: {
              'Content-Type': 'application/json',
            },
          }
        )
          .then((result) => {
            // Handle the API response if needed
            console.log('API Response - allow gamemode switch:', result);
          })
          .catch((error) => {
            // Handle errors
            console.error(
              'Error sending data to API - allow gamemode switch:',
              error
            );
          });
      });
      /*
    FORCE GAMEMODE
  */

      // Add event listener to all force buttons
      const forceButtons = document.querySelectorAll(
        '.table-button-force-gamemodes'
      );
      forceButtons.forEach((button) => {
        button.addEventListener('click', () => {
          console.log('Force Button clicked');
          console.log(`Force Button id: ${button.id} clicked`);
          var forceGamemodeId = button.id;

          // Get the corresponding checkbox for the current button
          const correspondingCheckbox = document.querySelector(
            `.set-available-checkbox[data-id="${forceGamemodeId}"]`
          );

          // Toggle the background color
          button.classList.toggle('gamemode-force-active');

          // Patch the new state to the API endpoint
          fetch(`${fetchdom}/gamemode/force/${forceGamemodeId}`, {
            method: 'PATCH',
            headers: {
              'Content-Type': 'application/json',
            },
          })
            .then((result) => {
              // Handle the API response if needed
              console.log('API Response - force gamemode:', result);
            })
            .catch((error) => {
              // Handle errors
              console.error(
                'Error sending data to API - force gamemode:',
                error
              );
            });

          // Ensure that only one force button is active at a time
          forceButtons.forEach((otherButton) => {
            if (otherButton !== button) {
              otherButton.classList.remove('gamemode-force-active');
            }
          });
        });
      });
    });
}
function handleButtons() {
  /*
    BUTTONS -  GET ALL BUTTONS
  */
  fetch(`${fetchdom}/button`)
    .then((response) => {
      if (!response.ok) {
        throw new Error(
          `Failed to fetch gamemodes. Status: ${response.status}`
        );
      }
      return response.json();
    })
    .then((buttons) => {
      console.log(buttons);
      let html = '<table class="rounded-table">';

      // Add table headers
      html += `
        <tr>
                    <th id="button-id">Id</th>
                    <th id="button-distance">Distance(m)</th>
                    <th id="button-actions">Actions</th>
                  </tr>
      `;

      for (const button of buttons) {
        html += `
          <tr data-button-id="${button.id}">
                    <td id="button-id-data">
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        width="24"
                        height="24"
                        viewBox="0 0 24 24"
                        fill="none"
                        stroke="currentColor"
                        stroke-width="2"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        class="lucide lucide-circle"
                      >
                        <circle cx="12" cy="12" r="10" />
                      </svg>
                    </td>
                    <td id="distance-data">${button.distance}</td>
                    <td>
                      <button class="table-button" onclick="editDistance(this)">
                        <svg
                          xmlns="http://www.w3.org/2000/svg"
                          width="24"
                          height="24"
                          viewBox="0 0 24 24"
                          fill="none"
                          stroke="currentColor"
                          stroke-width="2"
                          stroke-linecap="round"
                          stroke-linejoin="round"
                          class="lucide lucide-pencil editDistanceButtonsSVG"
                          data-state="state1"
                        >
                          <path
                            d="M17 3a2.85 2.83 0 1 1 4 4L7.5 20.5 2 22l1.5-5.5Z"
                          />
                          <path d="m15 5 4 4" />
                        </svg>
                      </button>
                      <button id="${button.id}" class="table-button" onclick="deleteButton(this)">
                        <svg
                          xmlns="http://www.w3.org/2000/svg"
                          width="24"
                          height="24"
                          viewBox="0 0 24 24"
                          fill="none"
                          stroke="currentColor"
                          stroke-width="2"
                          stroke-linecap="round"
                          stroke-linejoin="round"
                          class="lucide lucide-trash-2"
                        >
                          <path d="M3 6h18" />
                          <path d="M19 6v14c0 1-1 2-2 2H7c-1 0-2-1-2-2V6" />
                          <path d="M8 6V4c0-1 1-2 2-2h4c1 0 2 1 2 2v2" />
                          <line x1="10" x2="10" y1="11" y2="17" />
                          <line x1="14" x2="14" y1="11" y2="17" />
                        </svg>
                      </button>
                    </td>
                  </tr>`;
      }

      html += '</table>';
      buttonTable.innerHTML = html;
    });
}
function deleteButton(button) {
  // Get the parent <tr> element
  const tableRow = button.closest('tr');

  // Get the button ID from the data attribute
  const buttonId = tableRow.dataset.buttonId;

  // Confirm deletion with the user
  const confirmDelete = confirm('Are you sure you want to delete this button?');

  if (confirmDelete) {
    // Assuming you have an API endpoint for deleting a button
    const apiUrl = `${fetchdom}/button/${buttonId}`;

    // Make a DELETE request to the API endpoint
    fetch(apiUrl, {
      method: 'DELETE',
    })
      .then((data) => {
        console.log('Button deleted successfully:', data);
        tableRow.remove();
      })
      .catch((error) => {
        console.error('Error deleting button:', error);
      });
  }
}
function editDistance(button) {
  //CHANGE SVG

  // Get the SVG element inside the clicked button
  const svgElement = button.querySelector('.editDistanceButtonsSVG');

  // Check the current state based on the SVG content or class
  const currentState = svgElement.getAttribute('data-state');

  // Toggle between two states
  if (currentState === 'state1') {
    // Change to state 2
    svgElement.setAttribute('data-state', 'state2');
    svgElement.innerHTML = '<path d="M20 6 9 17l-5-5"/>'; // Change SVG path data for state 2
  } else {
    // Change to state 1
    svgElement.setAttribute('data-state', 'state1');
    svgElement.innerHTML =
      '<path d="M17 3a2.85 2.83 0 1 1 4 4L7.5 20.5 2 22l1.5-5.5Z"/><path d="m15 5 4 4"/>'; // Change SVG path data for state 1
  }

  //CHANGE DISTANCE TO INPUT FIELD

  // Get the parent <tr> element
  const tableRow = button.closest('tr');

  // Get the distance data <td> element and its current value
  const distanceTd = tableRow.querySelector('#distance-data');
  const currentDistance = distanceTd.textContent;

  // Check if the distance data is already in edit mode
  if (distanceTd.classList.contains('edit-mode')) {
    // Save the updated distance from the input
    const newDistance = distanceTd.querySelector('input').value;
    // Send to api
    updateDistanceOnAPI(tableRow.dataset.buttonId, newDistance);
    // Update the distance data and exit edit mode
    distanceTd.textContent = newDistance;
    distanceTd.classList.remove('edit-mode');
  } else {
    // Enter edit mode by replacing the distance data with an input field
    distanceTd.innerHTML = `<input type="text" style="width:25px" value="${currentDistance}" />`;
    distanceTd.classList.add('edit-mode');
  }
}
function updateDistanceOnAPI(buttonId, newDistance) {
  // Assuming you have an API endpoint for updating the distance
  const apiUrl = `${fetchdom}/button/${buttonId}?distance=${newDistance}`;

  // Make a PUT request to the API endpoint with the new distance data
  fetch(apiUrl, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
    },
  })
    .then((data) => {
      console.log('Distance updated successfully:', data);
    })
    .catch((error) => {
      console.error('Error updating distance:', error);
    });
}

function handleLightstrips() {
  /*
    LIGHTSTRIPS  -  GET ALL STRIPS
  */
  fetch(`${fetchdom}/lightstrip/settings`)
    .then((response) => {
      if (!response.ok) {
        throw new Error(
          `Failed to fetch lightstrips. Status: ${response.status}`
        );
      }
      return response.json();
    })
    .then((lightstrips) => {
      console.log(lightstrips);
      let html = '<table class="rounded-table">';

      // Add table headers
      html += `
        <tr>
                    <th>Distance(m)</th>
                    <th>Length(m)</th>
                    <th>Pixels</th>
                    <th id="lightstrip-action">Actions</th>
                  </tr>
      `;

      for (const strip of lightstrips.lightstrips) {
        console.log(strip);
        html += `
         <tr>
                    <td id="lightstrip-distance-data">${strip.distance}</td>
                    <td id="lightstrip-length-data">${strip.length}</td>
                    <td id="lightstrip-pixels-data">${strip.pixels}</td>
                    <td>
                      <button class="table-button">
                        <svg
                          xmlns="http://www.w3.org/2000/svg"
                          width="24"
                          height="24"
                          viewBox="0 0 24 24"
                          fill="none"
                          stroke="currentColor"
                          stroke-width="2"
                          stroke-linecap="round"
                          stroke-linejoin="round"
                          class="lucide lucide-pencil"
                        >
                          <path
                            d="M17 3a2.85 2.83 0 1 1 4 4L7.5 20.5 2 22l1.5-5.5Z"
                          />
                          <path d="m15 5 4 4" />
                        </svg>
                      </button>
                      <button class="table-button">
                        <svg
                          xmlns="http://www.w3.org/2000/svg"
                          width="24"
                          height="24"
                          viewBox="0 0 24 24"
                          fill="none"
                          stroke="currentColor"
                          stroke-width="2"
                          stroke-linecap="round"
                          stroke-linejoin="round"
                          class="lucide lucide-trash-2"
                        >
                          <path d="M3 6h18" />
                          <path d="M19 6v14c0 1-1 2-2 2H7c-1 0-2-1-2-2V6" />
                          <path d="M8 6V4c0-1 1-2 2-2h4c1 0 2 1 2 2v2" />
                          <line x1="10" x2="10" y1="11" y2="17" />
                          <line x1="14" x2="14" y1="11" y2="17" />
                        </svg>
                      </button>
                    </td>
                  </tr>`;
      }

      html += '</table>';
      lightstripsTable.innerHTML = html;
    });
}
function toggleDropdown() {
  var dropdown = document.getElementById('customDropdown');
  dropdown.style.display =
    dropdown.style.display === 'block' ? 'none' : 'block';
}

document.addEventListener('DOMContentLoaded', (event) => {
  //Gamemode vars
  gamemodesTable = document.querySelector('.js-gamemodes-table');
  buttonTable = document.querySelector('.js-buttons-table');
  lightstripsTable = document.querySelector('.js-lightstrips-table');
  //Functions
  checkUrl();
  checkAdmin();
  checkLogout();
  handleGamemodes();
  handleButtons();
  handleLightstrips();
});
