var gameMode;
var gameId;
var gamemodeSettingHeader;

var gameModeCardsContainer;

var currentSettings = [];
var fetchdom = 'http://localhost:5165';

function getParameters() {
  // Get the query string from the current URL
  const queryString = window.location.search;

  // Create a URLSearchParams object from the query string
  const urlParams = new URLSearchParams(queryString);

  // Get the value of the 'name' parameter
  gameMode = urlParams.get('name');
  gameId = urlParams.get('id');

  console.log(gameMode);
  console.log(gameId);
  gamemodeSettingHeader.innerHTML = `${gameMode} settings`;

  handleSettingsContent(gameId);
}
function handleSettingsContent(gameId) {
  fetch(`${fetchdom}/gamemode/settings/${gameId}`)
    .then((response) => {
      if (!response.ok) {
        throw new Error(
          `Failed to fetch gamemodes. Status: ${response.status}`
        );
      }
      return response.json();
    })
    .then((settings) => {
      // no settings = type 0  //time = type 1 // amount = type 2
      console.log(settings);
      console.log(settings.length);
      currentSettings = settings;
      let settingCardContainer = document.querySelector(
        '.gamemode-settings-card-container'
      );
      const timeInput = `<input
              type="number"
              id="playerInputTimeMinutes"
              class="playerInput"
              placeholder="Minutes"
              min="0"
              max="10"
            /><span>:</span
            ><input
              type="number"
              id="playerInputTimeSeconds"
              class="playerInput"
              placeholder="Seconds"
              min="0"
              max="60"
            />`;
      const amountInput = `<input
              type="number"
              id="playerInputAmount"
              class="playerInput"
              placeholder="Enter number of players"
              min="0"
            />`;
      let html = '';
      if (settings.length == 0) {
        // no setting
        html += `<div class="gamemode-settings-card js-no-setting">
        <button class="startButton" onclick="startGame()">Start</button>
      </div>`;
      } else {
        for (const setting of settings) {
          if (setting.type == 0) {
            //time;
            html += `${timeInput}<br>`;
            typeSetting = 1;
          }
          if (setting.type == 1) {
            //amount
            html += `${amountInput}<br>`;
            typeSetting = 2;
          }
        }
      }
      html += `<button class="startButton" onclick="startGame()">Start</button>`;
      settingCardContainer.innerHTML = html;
    });
}
function goBack() {
  window.location.href = 'index.html';
  fetch(`${fetchdom}/gamemode/stop`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
  })
    .then((result) => {
      // Handle the API response if needed
      console.log('API Response - send gamemode settings:', result);
    })
    .catch((error) => {
      // Handle errors
      console.error(
        'Error sending data to API - send gamemode settings:',
        error
      );
    });
}
function startGame() {
  let body = {};
  // Get the parameter from the input field
  var amountOfPlayers = document.getElementById('playerInputAmount').value;
  var inputTimeMinutes = document.getElementById(
    'playerInputTimeMinutes'
  ).value;
  var inputTimeSeconds = document.getElementById(
    'playerInputTimeSeconds'
  ).value;
  if (currentSettings.length == 0) {
    body = null;
  }
  for (const setting of currentSettings) {
    if (setting.type == 0) {
      // time
      body[setting.name] = `${inputTimeMinutes}:${inputTimeSeconds}`;
    }
    if (setting.type == 1) {
      // amount players
      body[setting.name] = amountOfPlayers;
    }
  }
  debugger;
  sendSettingToAPi(body);

  // Redirect to the new page with the parameter
  window.location.href = `gamemodeActive.html?name=${gameMode}`;
}
function sendSettingToAPi(body) {
  console.log(JSON.stringify(body));
  //Send gamemode settings when game starts
  fetch(`${fetchdom}/gamemode/current/${gameId}`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(body),
  })
    .then((result) => {
      // Handle the API response if needed
      console.log('API Response - send gamemode settings:', result);
    })
    .catch((error) => {
      // Handle errors
      console.error(
        'Error sending data to API - send gamemode settings:',
        error
      );
    });
}
document.addEventListener('DOMContentLoaded', function () {
  console.log('DOM loaded');
  gamemodeSettingHeader = document.querySelector('.js-gamemode-setting-header');
  gameModeCardsContainer = document.querySelector(
    '.js-gamemodes-card-container'
  );
  getParameters();
});
