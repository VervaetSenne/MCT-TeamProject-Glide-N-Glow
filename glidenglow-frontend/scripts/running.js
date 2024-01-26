var fetchdom = 'http://localhost:5165';

var gameMode;
var gamemodeSettingHeader;

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
    })
    .catch((error) => {
      // Handle errors
      console.error(
        'Error sending data to API - send gamemode settings:',
        error
      );
    });
  window.location.href = 'index.html';
}

document.addEventListener('DOMContentLoaded', function () {
  gamemodeSettingHeader = document.querySelector('.js-gamemode-setting-header');
  getParameters();
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
