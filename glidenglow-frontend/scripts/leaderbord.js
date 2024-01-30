var dropdownContainer;
var leaderbordTableContainer;

function handleDropdown() {
  fetch(`${fetchdom}/leaderboard/gamemodes`)
    .then((response) => {
      if (!response.ok) {
        throw new Error(
          `Failed to fetch gamemodes. Status: ${response.status}`
        );
      }
      return response.json();
    })
    .then((gamemodes) => {
      // Assuming your dropdown is a select element with class 'custom-select'
      var selectDropdown = document.querySelector('.custom-select select');

      // Clear existing options
      selectDropdown.innerHTML = '';

      // Add a default option as a placeholder
      var defaultOption = document.createElement('option');
      defaultOption.value = ''; // Set an appropriate value if needed
      defaultOption.text = 'Mode';
      defaultOption.disabled = true;
      defaultOption.selected = true;
      selectDropdown.appendChild(defaultOption);

      // Create and append options based on the retrieved gamemodes
      gamemodes.forEach((gamemode) => {
        var option = document.createElement('option');
        option.value = gamemode.id; // Set the value to the gamemode ID or relevant property
        option.text = gamemode.name; // Set the text to the gamemode name or relevant property
        selectDropdown.appendChild(option);
      });

      // Now, initialize the custom-select elements only once
      initializeCustomSelect();

      // Get the ID of the first gamemode
      var firstGamemodeId = gamemodes.length > 0 ? gamemodes[0].id : '';

      // Call fillLeaderbordOnChange with the ID of the first gamemode
      fillLeaderbordOnChange(firstGamemodeId);
    });
}

function initializeCustomSelect() {
  var x, i, j, l, ll, selElmnt, a, b, c;

  /* look for any elements with the class "custom-select": */
  x = document.getElementsByClassName('custom-select');
  l = x.length;
  for (i = 0; i < l; i++) {
    selElmnt = x[i].getElementsByTagName('select')[0];
    ll = selElmnt.length;
    /* for each element, create a new DIV that will act as the selected item: */
    a = document.createElement('DIV');
    a.setAttribute('class', 'select-selected');
    a.innerHTML = selElmnt.options[selElmnt.selectedIndex].innerHTML;
    x[i].appendChild(a);
    /* for each element, create a new DIV that will contain the option list: */
    b = document.createElement('DIV');
    b.setAttribute('class', 'select-items select-hide');
    for (j = 1; j < ll; j++) {
      /* for each option in the original select element,
      create a new DIV that will act as an option item: */
      c = document.createElement('DIV');
      c.innerHTML = selElmnt.options[j].innerHTML;
      c.addEventListener('click', function (e) {
        /* when an item is clicked, update the original select box,
          and the selected item: */
        var y, i, k, s, h, sl, yl;
        s = this.parentNode.parentNode.getElementsByTagName('select')[0];
        sl = s.length;
        h = this.parentNode.previousSibling;
        for (i = 0; i < sl; i++) {
          if (s.options[i].innerHTML == this.innerHTML) {
            // Log the selected ID
            fillLeaderbordOnChange(s.options[i].value);
            console.log('Selected ID:', s.options[i].value);

            s.selectedIndex = i;
            h.innerHTML = this.innerHTML;
            y = this.parentNode.getElementsByClassName('same-as-selected');
            yl = y.length;
            for (k = 0; k < yl; k++) {
              y[k].removeAttribute('class');
            }
            this.setAttribute('class', 'same-as-selected');
            break;
          }
        }
        h.click();
      });
      b.appendChild(c);
    }
    x[i].appendChild(b);
    a.addEventListener('click', function (e) {
      /* when the select box is clicked, close any other select boxes,
        and open/close the current select box: */
      e.stopPropagation();
      closeAllSelect(this);
      this.nextSibling.classList.toggle('select-hide');
      this.classList.toggle('select-arrow-active');
    });
  }
}

function fillLeaderbordOnChange(gamemodeId) {
  fetch(`${fetchdom}/leaderboard/${gamemodeId}`)
    .then((response) => {
      if (!response.ok) {
        throw new Error(
          `Failed to fetch leaderboards. Status: ${response.status}`
        );
      }
      return response.json();
    })
    .then((scores) => {
      if (scores.length == 1) {
        document.querySelector('.first-place h1').innerText = scores[0].score;
        document.querySelector('.first-place p').innerText = scores[0].username;

        document.querySelector('.second-place h1').innerText = '';
        document.querySelector('.second-place p').innerText = '';

        document.querySelector('.third-place h1').innerText = '';
        document.querySelector('.third-place p').innerText = '';
      }
      if (scores.length == 2) {
        document.querySelector('.first-place h1').innerText = scores[0].score;
        document.querySelector('.first-place p').innerText = scores[0].username;

        document.querySelector('.second-place h1').innerText = scores[1].score;
        document.querySelector('.second-place p').innerText =
          scores[1].username;

        document.querySelector('.third-place h1').innerText = '';
        document.querySelector('.third-place p').innerText = '';
      }
      if (scores.length == 3) {
        // Fill the podium with the first 3 scores
        document.querySelector('.first-place h1').innerText = scores[0].score;
        document.querySelector('.first-place p').innerText = scores[0].username;

        document.querySelector('.second-place h1').innerText = scores[1].score;
        document.querySelector('.second-place p').innerText =
          scores[1].username;

        document.querySelector('.third-place h1').innerText = scores[2].score;
        document.querySelector('.third-place p').innerText = scores[2].username;
      }
      let html = `<table>`;
      html += `<tr>
            <th>Rank</th>
            <th>Username</th>
            <th>Score</th>
          </tr>`;
      for (const score of scores) {
        html += `
        <tr>
            <td>${score.rank}</td>
            <td>${score.username}</td>
            <td>${score.score}</td>
          </tr>`;
      }
      html += `</table>`;
      leaderbordTableContainer.innerHTML = html;
    })
    .catch((error) => {
      console.error('Error fetching leaderboards:', error.message);
    });
}

function defaultLeaderbordContent() {}

function closeAllSelect(elmnt) {
  /*a function that will close all select boxes in the document,
    except the current select box:*/
  var x,
    y,
    i,
    xl,
    yl,
    arrNo = [];
  x = document.getElementsByClassName('select-items');
  y = document.getElementsByClassName('select-selected');
  xl = x.length;
  yl = y.length;
  for (i = 0; i < yl; i++) {
    if (elmnt == y[i]) {
      arrNo.push(i);
    } else {
      y[i].classList.remove('select-arrow-active');
    }
  }
  for (i = 0; i < xl; i++) {
    if (arrNo.indexOf(i)) {
      x[i].classList.add('select-hide');
    }
  }
}
document.addEventListener('click', closeAllSelect);

document.addEventListener('DOMContentLoaded', function () {
  console.log('DOM loaded');
  dropdownContainer = document.querySelector('.js-gamemode-dropdown');
  defaultLeaderbordContent();
  handleDropdown();
  leaderbordTableContainer = document.querySelector('.js-table-container');
});
