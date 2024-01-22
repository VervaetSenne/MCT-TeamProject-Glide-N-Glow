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
  gamemodeSettingHeader.innerHTML = `${gameMode} settings`;

  if (gameMode == 'TimeTrial') {
    document
      .querySelector('.js-player-amount')
      .classList.add('gamemode-setting-content-hidden');
    document
      .querySelector('.js-set-time')
      .classList.remove('gamemode-setting-content-hidden');
  }
  if (gameMode == 'TimeTrial') {
    document
      .querySelector('.js-player-amount')
      .classList.add('gamemode-setting-content-hidden');
    document
      .querySelector('.js-set-time')
      .classList.remove('gamemode-setting-content-hidden');
  }
  if (gameMode == 'Collect') {
    document
      .querySelector('.js-player-amount')
      .classList.remove('gamemode-setting-content-hidden');
    document
      .querySelector('.js-set-time')
      .classList.add('gamemode-setting-content-hidden');
  }
}
function goBack() {
  window.location.href = 'index.html';
}
function startGame() {
  // Get the parameter from the input field
  var playersCount = document.getElementById('playerInput').value;

  // Validate that the playersCount is not negative
  if (playersCount < 0) {
    alert('Players count cannot be negative!');
    return;
  }

  // Redirect to the new page with the parameter
  window.location.href = `gamemodeActive.html?name=${gameMode}`;
}
document.addEventListener('DOMContentLoaded', function () {
  gamemodeSettingHeader = document.querySelector('.js-gamemode-setting-header');
  getParameters();
});
