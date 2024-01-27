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

      let html = `<div class="gamemode-settings-card`;
      if (settings.length == 0) {
        // no setting
        html += `js-no-setting">`;
      } else {
        html += `">`;
        for (const setting of settings) {
          if (setting.type == 0) {
            //time;
            html += `<p>Set time</p>`;
            html += `<input
              type="number"
              id="input_${setting.name}_minutes"
              class="playerInput"
              placeholder="Minutes"
              min="0"
              max="10"
              value ="0"
            /><span>:</span
            ><input
              type="number"
              id="input_${setting.name}_seconds"
              class="playerInput"
              placeholder="Seconds"
              min="0"
              max="60"
              value="0"
            /><br>`;
          }
          if (setting.type == 1) {
            //amount
            html += `<p>Amount of players</p>`;
            html += `<input
              type="number"
              id="input_${setting.name}"
              class="playerInput"
              placeholder="Enter number of players"
              min="0"
              max="5"
              value="0"
            /><br>`;
          }
        }
      }
      html += `<button class="startButton" onclick="startGame()">Start</button>`;
      html += `</div>`;
      settingCardContainer.innerHTML = html;
    });
}
function goBack() {
  window.location.href = 'index.html';
}
function startGame() {
  let body = {};
  // Get the parameter from the input field
  if (currentSettings.length == 0) {
    body = null;
  }
  for (const setting of currentSettings) {
    if (setting.type == 0) {
      // time
      body[setting.name] = `${
        document.getElementById(`input_${setting.name}_minutes`).value
      }:${document.getElementById(`input_${setting.name}_seconds`).value}`;
    }
    if (setting.type == 1) {
      // amount players
      body[setting.name] = document.getElementById(
        `input_${setting.name}`
      ).value;
    }
  }
  sendSettingToAPi(
    body,
    () => (window.location.href = `gamemodeActive.html?name=${gameMode}`)
  );
}
function sendSettingToAPi(body, action) {
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
      action();
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
