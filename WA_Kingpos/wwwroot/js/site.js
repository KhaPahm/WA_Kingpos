// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
//function HideOrUnhide() {
//    var x = document.getElementById("GridView");
//    if (x.style.display === "none") {
//        x.style.display = "block";
//    } else {
//        x.style.display = "none";
//    }

//    var y = document.getElementById("CardView");
//    if (y.style.display === "none") {
//        y.style.display = "block";
//        x.style.display = "none";
//    } else {
//        y.style.display = "none";
//        x.style.display = "block";
//    }
//}

//function CallHideGrid() {
//    var x = document.getElementById("GridView");
//    var y = document.getElementById("CardView");
//    x.style.display = "none";
//    y.style.display = "block";
//}

//function HideOrUnhide() {
//    var x = document.getElementById("GridView");
//    if (x.hidden == true) {
//        x.hidden = false;
//    } else {
//        x.hidden = true;
//    }

//    var y = document.getElementById("CardView");
//    if (y.hidden == true) {
//        y.hidden = false;
//        x.hidden = true;
//    } else {
//        y.hidden = true;
//        x.hidden = false;
//    }
//}

//function CallHideGrid() {
//    var x = document.getElementById("GridView");
//    var y = document.getElementById("CardView");
//    x.hidden = true;
//    y.hidden = false;
//}

function HideOrUnhide() {
    var x = document.getElementById("GridView");
    if (x.style.visibility === "hidden") {
        x.style.visibility = "visible";
        x.style.height = "auto";
        x.style.marginTop = "15px"
    } else {
        x.style.visibility = "hidden";
        x.style.height = "0%";
        x.style.marginTop = "0px";
    }

    var y = document.getElementById("CardView");
    if (y.style.visibility === "hidden") {
        y.style.visibility = "visible";
        y.hidden = false;

        x.style.visibility = "hidden";
        x.style.height = "0%";
        x.style.marginTop = "0px";
    } else {
        y.style.visibility = "hidden";
        y.hidden = true;

        x.style.visibility = "visible";
        x.style.height = "auto";
        x.style.marginTop = "15px";
    }
}

function CallHideGrid() {
    var x = document.getElementById("GridView");
    var y = document.getElementById("CardView");
    x.style.visibility = "hidden";
    x.style.height = "0%";
    x.style.marginTop = "0px";

    y.style.visibility = "visible";
    y.hidden = false;
}

$(document).ready(function () {
    $('#tablesortqc').DataTable({
        "order": [[0, "asc"]]  // Default sort by first column (ascending)
    });
});

window.addEventListener("DOMContentLoaded", CallHideGrid());