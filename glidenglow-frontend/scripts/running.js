var fetchdom = 'http://localhost:5165';

var gameMode;
var gamemodeSettingHeader;

var userContentContainerl;

function getParameters() {
  // Get the query string from the current URL
  const queryString = window.location.search;

  // Create a URLSearchParams object from the query string
  const urlParams = new URLSearchParams(queryString);

  // Get the value of the 'name' parameter
  gameMode = urlParams.get('name');

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
  fetch(`${fetchdom}/running/scores`)
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
      for (const content of userContent) {
        //0 time // 1 = value opvragen player cards
        console.log(content);
        if (content.type == 0) {
          //show timer
        }
        if (content.type == 1) {
          //show player cards
        }
      }
    });
}

document.addEventListener('DOMContentLoaded', function () {
  gamemodeSettingHeader = document.querySelector('.js-gamemode-setting-header');
  userContentContainer = document.querySelector('.js-content-container');
  getParameters();
  loadUserContent();
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
