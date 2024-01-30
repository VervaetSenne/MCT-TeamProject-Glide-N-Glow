//signalR
var gameHubConnection;

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
        window.location.href = `/gamemodeActive.html?name=${currentGame.name}&id=${currentGame.id}`;
      }
    });
}

document.addEventListener('DOMContentLoaded', (event) => {
  checkCurrentGame();

  gameHubConnection = new signalR.HubConnectionBuilder()
  .withUrl(fetchdom + "/game-hub")
  .configureLogging(signalR.LogLevel.Warning)
  .build();

  gameHubConnection.on("current-game-updated", (gameid) => {
    if (gameid){
      checkCurrentGame();
    }
    else{
      console.log('the game has ended');
      window.location.href = '/';
    }
  });
  
  gameHubConnection
    .start()
    .then(function () {
      console.log('SignalR connected');
    })
    .catch(function (err) {
      console.error('Error connecting to SignalR:', err);
    });
});
