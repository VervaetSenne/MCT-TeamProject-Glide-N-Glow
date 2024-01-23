var gameMode;
var gameId;
var gamemodeSettingHeader;

var gameModeCardsContainer;

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

  if (gameMode == 'TimeRace') {
    document
      .querySelector('.js-no-setting')
      .classList.add('gamemode-setting-content-hidden');
    document
      .querySelector('.js-set-time')
      .classList.remove('gamemode-setting-content-hidden');
    document
      .querySelector('.js-player-amount')
      .classList.add('gamemode-setting-content-hidden');
  }
  if (gameMode == 'TimeTrial') {
    document
      .querySelector('.js-player-amount')
      .classList.add('gamemode-setting-content-hidden');
    document
      .querySelector('.js-set-time')
      .classList.add('gamemode-setting-content-hidden');
    document
      .querySelector('.js-no-setting')
      .classList.remove('gamemode-setting-content-hidden');
  }
  if (gameMode == 'Collect') {
    document
      .querySelector('.js-player-amount')
      .classList.remove('gamemode-setting-content-hidden');
    document
      .querySelector('.js-set-time')
      .classList.add('gamemode-setting-content-hidden');
    document
      .querySelector('.js-no-setting')
      .classList.add('gamemode-setting-content-hidden');
  }
}
function goBack() {
  window.location.href = 'index.html';
}
function startGame() {
  // Get the parameter from the input field
  var amountOfPlayers = document.getElementById('playerInputAmount').value;
  var inputTimeMinutes = document.getElementById(
    'playerInputTimeMinutes'
  ).value;
  var inputTimeSeconds = document.getElementById(
    'playerInputTimeSeconds'
  ).value;

  //Send gamemode settings when game starts
  fetch(`${fetchdom}/gamemode/current/${gameId}`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      body: JSON.stringify(amountOfPlayers, inputTimeMinutes, inputTimeSeconds),
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

  // Redirect to the new page with the parameter
  window.location.href = `gamemodeActive.html?name=${gameMode}`;
}

function handleGamemodes() {
  /*
    GAMEMODES GET ALL GAMEMODES
  */
  console.log('handle gamemodes function');
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
      let html = '';
      for (const gamemode of gamemodes.gamemodes) {
        console.log(gamemode);
        html += `
          <a href="gamemode.html?name=${gamemode.name}&id=${gamemode.id}" class="gamemode-card">
          <div class="gamemode-card-banner">
            <img
              src="img/gamemode-img.jpg"
              alt="gamemode card picture"
              class="gamemode-card-banner-img"
            />
          </div>
          <div class="gamemode-card-info">
            <p class="gamemode-name">${gamemode.name}</p>
            <p class="gamemode-discription">
              ${gamemode.description}
            </p>
            <div class="gamemode-card-best-time">
              <svg
                xmlns="http://www.w3.org/2000/svg"
                width="24"
                height="24"
                viewBox="0 0 24 24"
                fill="none"
                stroke="#000000"
                stroke-width="2"
                stroke-linecap="round"
                stroke-linejoin="round"
                class="lucide lucide-trophy"
              >
                <path d="M6 9H4.5a2.5 2.5 0 0 1 0-5H6" />
                <path d="M18 9h1.5a2.5 2.5 0 0 0 0-5H18" />
                <path d="M4 22h16" />
                <path
                  d="M10 14.66V17c0 .55-.47.98-.97 1.21C7.85 18.75 7 20.24 7 22"
                />
                <path
                  d="M14 14.66V17c0 .55.47.98.97 1.21C16.15 18.75 17 20.24 17 22"
                />
                <path d="M18 2H6v7a6 6 0 0 0 12 0V2Z" />
              </svg>
              <p>Best score: <span>1:30</span></p>
            </div>
          </div>
        </a>`;
      }

      html += '</table>';
      gameModeCardsContainer.innerHTML = html;
    });
}
document.addEventListener('DOMContentLoaded', function () {
  console.log('DOM loaded');
  gamemodeSettingHeader = document.querySelector('.js-gamemode-setting-header');
  gameModeCardsContainer = document.querySelector(
    '.js-gamemodes-card-container'
  );
  handleGamemodes();
  getParameters();
});
