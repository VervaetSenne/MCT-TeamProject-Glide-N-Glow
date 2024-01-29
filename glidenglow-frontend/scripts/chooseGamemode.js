var fetchdom = 'http://localhost:5165';

//gamemode cards var

var gameModeCardsContainer;

function handleGamemodeCards() {
  /*
    GAMEMODES GET ALL GAMEMODES CARDS FILLED
  */
  console.log('handle gamemodes function');
  fetch(`${fetchdom}/gamemode/available`)
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
      let bestScore = 0;
      for (const gamemode of gamemodes) {
        console.log(gamemode);
        bestScore = gamemode.bestScore;
        if (bestScore == '----') bestScore = '';
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
              <p>Best score: <span>${bestScore}</span></p>
            </div>
          </div>
        </a>`;
      }
      gameModeCardsContainer.innerHTML = html;
    });
}

document.addEventListener('DOMContentLoaded', (event) => {
  //Gamemode vars
  gameModeCardsContainer = document.querySelector(
    '.js-gamemodes-card-container'
  );
  //Functions
  handleGamemodeCards();
});
