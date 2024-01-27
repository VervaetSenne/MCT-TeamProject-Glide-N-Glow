var fetchdom = 'http://localhost:5165';

function checkCurrentGame() {
  fetch(`${fetchdom}/gamemode/current`)
    .then((response) => {
      if (!response.ok) {
        throw new Error(
          `Failed to fetch gamemodes. Status: ${response.status}`
        );
      }
      if (response.status == 204) {
        return 0;
      }
      return response.json();
    })
    .then((currentGame) => {
      if (currentGame != 0) {
        console.log('there is a game in progress');
        window.location.href = `/gamemodeActive.html?name=${currentGame.name}`;
      }
    });
}

document.addEventListener('DOMContentLoaded', (event) => {
  checkCurrentGame();
});
