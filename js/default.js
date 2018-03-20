$(document).ready(function() {
    convertToMB();
    formatChangelog();
});
  
function convertToMB(){
    var element1 = document.getElementById("size-text")
    var num = element1.textContent;
    var n=num.slice(6);
    var number = Number(n);
    number = number/1024;
    number = (number / 1024).toFixed(2);
    var numberString = String(number);
    element1.innerHTML = "Size: ".concat(numberString," MB");
}

function formatChangelog() {
    var element2 = document.getElementById("latestChanges")
    var log = "";
    $.get('./assets/latestChanges.txt', function(data) {
      log = data.replace(/(?:\r\n|\r|\n)/g, '<br />');
      element2.innerHTML = log;
    }, 'text');
    
    var element3 = document.getElementById("changelogModalLabel");
    var changelogModelTitle = element3.textContent;
    $.get('./assets/latestReleaseDate.txt', function(data2) {
      var d = new Date(Date.parse(data2));
      var shortDate = String(d.getDate()).concat("-",String(d.getMonth()),"-",String(d.getFullYear())); 
      element3.innerHTML = changelogModelTitle + " (" + shortDate + ")";
    }, 'text');
  }

function thanks() {
    setTimeout(function () {
        document.location.pathname = "WinHue3/download.html";
    }, 5000);
}
