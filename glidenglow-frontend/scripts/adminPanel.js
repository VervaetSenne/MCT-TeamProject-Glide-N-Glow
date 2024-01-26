// Check if the user is logged in as admin
const isAdminLoggedIn = localStorage.getItem('AdminLoggedIn') === 'true';
var fetchdom = 'http://localhost:5165';

//Gamemodes var
let gamemodesTable;
let AllowGamemodeSwitchLoadState;
let allowGamemodeSwitchState;

//Buttons var
let buttonTable;

//Lightstrips var
let lightstripsTable;

//gamemode cards var

var gameModeCardsContainer;

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
    checkAdmin();
    console.log('logout');
  });
}
function handleAdminSettings() {
  var startStopButton = document.getElementById('settings-startstop-button');
  var calibrateButton = document.getElementById('settings-callibrate-button');
  var calibrateDiv = document.querySelectorAll('.hidey');
  var stateStartStop = 0;
  var stateCalibrate = 0;

  startStopButton.addEventListener('click', function () {
    console.log('startstop');
    stateStartStop++;
    //Stop games
    if (stateStartStop == 1) {
      startStopButton.style.backgroundColor = 'green';
      startStopButton.innerHTML = 'Turn lights on';
      fetch(`${fetchdom}/gamemode/stop/`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ stop: true }),
      })
        .then((result) => {
          // Handle the API response if needed
          console.log('API Response - START STOP GAMEMODE:', result);
        })
        .catch((error) => {
          // Handle errors
          console.error(
            'Error sending data to API - START STOP GAMEMODE:',
            error
          );
        });
    }
    //start again
    if (stateStartStop == 2) {
      startStopButton.style.backgroundColor = '#9b2d2d';
      startStopButton.innerHTML = 'Turn lights off';
      fetch(`${fetchdom}/gamemode/stop/`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ stop: false }),
      })
        .then((result) => {
          // Handle the API response if needed
          console.log('API Response - START STOP GAMEMODE:', result);
        })
        .catch((error) => {
          // Handle errors
          console.error(
            'Error sending data to API - START STOP GAMEMODE:',
            error
          );
        });
      stateStartStop = 0;
    }
  });
  calibrateButton.addEventListener('click', function () {
    console.log('callibrate');
    stateCalibrate++;
    if (stateCalibrate == 1) {
      calibrateDiv.forEach((div) => {
        div.classList.add('gamemode-setting-content-hidden');
      });
      calibrateButton.innerHTML = 'Stop callibrate';
      startStopButton.style.opacity = '0.5';
      startStopButton.disabled = true;
    }
    if (stateCalibrate == 2) {
      calibrateDiv.forEach((div) => {
        div.classList.remove('gamemode-setting-content-hidden');
      });
      calibrateButton.innerHTML = 'Callibrate';
      startStopButton.style.opacity = '1';
      startStopButton.disabled = false;
      stateCalibrate = 0;
    }
  });
}
function handleGamemodes() {
  /*
    GAMEMODES GET ALL GAMEMODES
  */
  console.log('gamemodes function');
  console.log(window.location.hostname);
  fetch(`${fetchdom}/gamemode/admin-settings`)
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
        allowGamemodeSwitchState = gamemodes.allowUserSwitching;
        html += `
          <tr>
            <td>${gamemode.name}</td>
            <td>
              <label class="checkbox-container">
                <input type="checkbox" class="set-available-checkbox" ${
                  gamemode.available ? 'checked' : ''
                } data-id="${gamemode.id}" />

                <span class="checkmark"></span>
              </label>
            </td>
            <td>
              <button class="table-button table-button-force-gamemodes" id="${
                gamemode.id
              }">
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
      allowGamemodeToggle.checked = allowGamemodeSwitchState;
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
    GET ALL BUTTONS
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
                        fill=#${button.id}
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
                      <button class="table-button" onclick="editButtonDistance(this)">
                        <svg
                          xmlns="http://www.w3.org/2000/svg"
                          width="24"
                          height="24"
                          viewBox="0 0 24 24"
                          fill=""
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
function editButtonDistance(button) {
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
         <tr data-lightstrip-id="${strip.id}">
                    <td id="lightstrip-distance-data">${strip.distance}</td>
                    <td id="lightstrip-length-data">${strip.length}</td>
                    <td id="lightstrip-pixels-data">${strip.pixels}</td>
                    <td>
                      <button class="table-button" onclick="editLightstripData(this)">
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
                          class="lucide lucide-pencil editLightstripDataSVG"
                          data-state="state1"
                        >
                          <path
                            d="M17 3a2.85 2.83 0 1 1 4 4L7.5 20.5 2 22l1.5-5.5Z"
                          />
                          <path d="m15 5 4 4" />
                        </svg>
                      </button>
                      <button class="table-button" onclick="deleteLightstrip(this)">
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

      html += `<tr class="add-lightstrip-button">
                    <td></td>
                    <td>
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        width="34"
                        height="34"
                        viewBox="0 0 24 24"
                        fill="none"
                        stroke="currentColor"
                        stroke-width="2"
                        stroke-linecap="round"
                        stroke-linejoin="round"
                        class="lucide lucide-plus-circle ledstrip-add-icon"
                      >
                        <circle cx="12" cy="12" r="10" />
                        <path d="M8 12h8" />
                        <path d="M12 8v8" />
                      </svg>
                    </td>
                    <td></td>
                    <td></td>
                  </tr></table>`;
      lightstripsTable.innerHTML = html;

      // Add event listener for adding a lightstrip
      var addLightstripButton = document.querySelector('.ledstrip-add-icon');
      addLightstripButton.addEventListener('click', function () {
        console.log('add lightstrip');

        // Add a new row with initial values of 0 above the "Add Lightstrip" button
        const newRow = document.createElement('tr');
        newRow.innerHTML = `
    <td id="lightstrip-distance-data">0</td>
    <td id="lightstrip-length-data">0</td>
    <td id="lightstrip-pixels-data">0</td>
    <td>
      <button class="table-button" onclick="editLightstripData(this)">
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
                          class="lucide lucide-pencil editLightstripDataSVG"
                          data-state="state1"
                        >
                          <path
                            d="M17 3a2.85 2.83 0 1 1 4 4L7.5 20.5 2 22l1.5-5.5Z"
                          />
                          <path d="m15 5 4 4" />
                        </svg>
      </button>
      <button class="table-button" onclick="deleteLightstrip(this)">
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
  `;

        // Find the correct parent node for insertion
        const addLightstripButtonRow = addLightstripButton.closest('tr');

        // Insert the new row above the "Add Lightstrip" button
        addLightstripButtonRow.parentNode.insertBefore(
          newRow,
          addLightstripButtonRow
        );

        // Now make the API call to actually add the lightstrip
        fetch(`${fetchdom}/lightstrip?samePiece=${false}&onePiece=${false}`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({ stop: true }),
        })
          .then((result) => {
            // Handle the API response if needed
            console.log('API Response - ADD LIGHTSTRIP:', result);
          })
          .catch((error) => {
            // Handle errors
            console.error('Error sending data to API - ADD LIGHTSTRIP:', error);
          });
      });
    });
}
function editLightstripData(button) {
  console.log('editLightstripData');
  //CHANGE SVG

  // Get the SVG element inside the clicked button
  const svgElement = button.querySelector('.editLightstripDataSVG');

  // Check the current state based on the SVG content or class
  const currentState = svgElement.getAttribute('data-state');

  // Toggle between two states
  if (currentState === 'state1') {
    // Change to state 2 - EDIT MODE SVG
    svgElement.setAttribute('data-state', 'state2');
    svgElement.innerHTML = '<path d="M20 6 9 17l-5-5"/>'; // Change SVG path data for state 2
  } else {
    // Change to state 1 - ORIGINAL SVG
    svgElement.setAttribute('data-state', 'state1');
    svgElement.innerHTML =
      '<path d="M17 3a2.85 2.83 0 1 1 4 4L7.5 20.5 2 22l1.5-5.5Z"/><path d="m15 5 4 4"/>'; // Change SVG path data for state 1
  }

  // Get the parent <tr> element
  const tableRow = button.closest('tr');

  // Get the lightstrip ID from the data attribute
  const lightstripId = tableRow.dataset.lightstripId;

  // Object to store changes
  const changes = {};

  // Edit distance
  editLightstripField(
    tableRow,
    '#lightstrip-distance-data',
    lightstripId,
    'distance',
    changes
  );

  // Edit length
  editLightstripField(
    tableRow,
    '#lightstrip-length-data',
    lightstripId,
    'length',
    changes
  );

  // Edit pixels
  editLightstripField(
    tableRow,
    '#lightstrip-pixels-data',
    lightstripId,
    'pixels',
    changes
  );

  // Make a single API request with all changes
  if (Object.keys(changes).length > 0) {
    updateLightstripDataOnAPI(lightstripId, changes);
  }
}

function editLightstripField(
  tableRow,
  fieldSelector,
  lightstripId,
  field,
  changes
) {
  // Get the field data <td> element and its current value
  const fieldTd = tableRow.querySelector(fieldSelector);
  const currentFieldValue = fieldTd.textContent;

  // Check if the field data is already in edit mode
  if (fieldTd.classList.contains('edit-mode')) {
    // Save the updated field value from the input
    const newFieldValue = fieldTd.querySelector('input').value;

    // Store the change in the 'changes' object
    changes[field] = newFieldValue;

    // Update the field data and exit edit mode
    fieldTd.textContent = newFieldValue;
    fieldTd.classList.remove('edit-mode');
  } else {
    // Enter edit mode by replacing the field data with an input field
    fieldTd.innerHTML = `<input type="text" style="width:25px" value="${currentFieldValue}" />`;
    fieldTd.classList.add('edit-mode');
  }
}

function updateLightstripDataOnAPI(lightstripId, changes) {
  const apiUrl = `${fetchdom}/lightstrip/${lightstripId}`;
  // Make a PUT request to the API endpoint with the new lightstrip data
  console.log(changes);
  fetch(apiUrl, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(changes),
  })
    .then((data) => {
      console.log('Lightstrip data updated successfully:', data);
    })
    .catch((error) => {
      console.error('Error updating lightstrip data:', error);
    });
}

function deleteLightstrip(button) {
  // Get the parent <tr> element
  const tableRow = button.closest('tr');

  // Get the lightstrip ID from the data attribute
  const lightstripId = tableRow.dataset.lightstripId;

  // Confirm deletion with the user
  const confirmDelete = confirm(
    'Are you sure you want to delete this lightstrip?'
  );

  if (confirmDelete) {
    const apiUrl = `${fetchdom}/lightstrip/${lightstripId}`;

    // Make a DELETE request to the API endpoint
    fetch(apiUrl, {
      method: 'DELETE',
    })
      .then((data) => {
        console.log('Lightstrip deleted successfully:', data);
        tableRow.remove();
      })
      .catch((error) => {
        console.error('Error deleting lightstrip:', error);
      });
  }
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
  gameModeCardsContainer = document.querySelector(
    '.js-gamemodes-card-container'
  );
  //Functions
  checkUrl();
  checkAdmin();
  checkLogout();
  handleGamemodes();
  handleButtons();
  handleLightstrips();
  handleAdminSettings();
});
