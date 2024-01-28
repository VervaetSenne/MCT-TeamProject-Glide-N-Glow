var fetchdom = 'http://localhost:5165';

var gameMode;
var gameId;

var gamemodeSettingHeader;

var userContentContainerl;

var recentScoresContainer;

var connection;

function getParameters() {
  // Get the query string from the current URL
  const queryString = window.location.search;

  // Create a URLSearchParams object from the query string
  const urlParams = new URLSearchParams(queryString);

  // Get the value of the 'name' parameter
  gameMode = urlParams.get('name');
  gameId = urlParams.get('id');

  console.log(gameMode);
  gamemodeSettingHeader.innerHTML = `${gameMode} in progress`;
}

function goBack() {
  fetch(`${fetchdom}/gamemode/stop`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
  })
    .then((result) => {
      // Handle the API response if needed
      console.log('API Response - send gamemode settings:', result);
      window.location.href = 'index.html';
    })
    .catch((error) => {
      // Handle errors
      console.error(
        'Error sending data to API - send gamemode settings:',
        error
      );
    });
}
function loadUserContent() {
  fetch(`${fetchdom}/running/content/${gameId}`)
    .then((response) => {
      if (!response.ok) {
        throw new Error(
          `Failed to fetch gamemodes. Status: ${response.status}`
        );
      }
      return response.json();
    })
    .then((userContent) => {
      console.log(userContent);
      let html = ``;
      if (userContent.type == 0) {
        html += ``;
      } else if (userContent.type == 1) {
        for (let i = 0; i < userContent.value; i++) {
          html += `<div class="player-card">
      <div class="player-card-header">
        <p>Player ${i + 1}</p>
      </div>
      <div class="player-score-container">
        <p class="player-score-text">Score:</p>
        <p class="player-score player-score-${i}">0</p>
      </div>
    </div>`;
        }
      }
      userContentContainer.innerHTML = html;
    });
}

function handleRecentScores() {
  fetch(`${fetchdom}/running/scores`)
    .then((response) => {
      if (!response.ok) {
        throw new Error(
          `Failed to fetch gamemodes. Status: ${response.status}`
        );
      }
      return response.json();
    })
    .then((recentScores) => {
      console.log('recentScores');
      console.log(recentScores);
      let html = ``;
      html += `<tr>
        <th>Username</th>
        <th>Score</th>
        <th>`;
      for (const score of recentScores) {
        html += `<tr id="score-row-id-${score.playerIndex}">
        <td>${score.playerName}</td>
        <td>${score.value}</td>
        <td>
          <button class="table-button" id="${score.playerIndex}" onclick="claimScore(this)">
            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-hand">
              <path d="M18 11V6a2 2 0 0 0-2-2v0a2 2 0 0 0-2 2v0" />
              <path d="M14 10V4a2 2 0 0 0-2-2v0a2 2 0 0 0-2 2v2" />
              <path d="M10 10.5V6a2 2 0 0 0-2-2v0a2 2 0 0 0-2 2v8" />
              <path d="M18 8a2 2 0 1 1 4 0v6a8 8 0 0 1-8 8h-2c-2.8 0-4.5-.86-5.99-2.34l-3.6-3.6a2 2 0 0 1 2.83-2.82L7 15" />
            </svg>
          </button>
        </td>
      </tr>`;
      }
    });
}

function claimScore(button) {
  const buttonId = button.id;

  // Make a POST request to claim the score
  fetch(`${fetchdom}/running/score/${buttonId}`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
  })
    .then((result) => {
      // Handle the response if needed
      console.log('Score claimed successfully:', result);
    })
    .catch((error) => {
      // Handle errors
      console.error('Error claiming score:', error);
    });
}

document.addEventListener('DOMContentLoaded', function () {
  // Initialize SignalR connection
  connection = new signalR.HubConnectionBuilder()
    .withUrl(`${fetchdom}/game-hub`) // SignalR hub URL
    .build();

  connection.on('score-updated', function (playerIndex, newScore) {
    // Update the player score
    const playerScoreElement = document.getElementById(
      `player-score-${playerIndex}`
    );
    if (playerScoreElement) {
      playerScoreElement.innerText = newScore;
    }
  });

  connection.on('score-claimed', function (playerIndex, playerName) {
    //change row that has been clicked to claim
    const scoreRowElement = document.getElementById(
      `score-row-id-${playerIndex}`
    );
    if (scoreRowElement) {
      const playerNameElement = scoreRowElement.querySelector('td:first-child');

      if (playerNameElement) {
        playerNameElement.textContent = playerName;
      }
    }
  });

  connection.on('new-scores', function (playerIndex, playerName) {
    //add new score to row
  });

  connection
    .start()
    .then(function () {
      console.log('SignalR connected');
    })
    .catch(function (err) {
      console.error('Error connecting to SignalR:', err);
    });

  gamemodeSettingHeader = document.querySelector('.js-gamemode-setting-header');
  userContentContainer = document.querySelector('.js-content-container');
  recentScoresContainer = document.querySelector('.js-recent-scores');
  getParameters();
  loadUserContent();
  handleRecentScores();
  const animationContainer = document.getElementById('lottie-container');

  const animationData = {
    container: animationContainer,
    renderer: 'svg',
    loop: true,
    autoplay: true,
    path: '../lotti/lottiRunning.json',
  };

  const animation = lottie.loadAnimation(animationData);
});
