function showToast(message, title = "", type = "success", timeShow = 4000) {
    const toastEl = document.getElementById('appToast');
    const toastBody = document.getElementById('appToastBody');
    const toastHeaderTitle = document.getElementById('appToastHeaderTitle');

    // Reset body
    toastBody.innerHTML = "";

    // Create text span
    const text = document.createElement("span");
    text.className = "fw-semibold text-wrap text-dark"; // make text a bit bolder
    text.innerHTML = message
    // Create icon element
    const icon = document.createElement("i");
    if (type === "success") {
        icon.className = "fas fa-check-circle mr-2 text-success";
    } else if (type === "error") {
        icon.className = "fas fa-times-circle text-danger mr-2 text-danger";
    } else if (type === "warning") {
        icon.className = "fas fa-exclamation-triangle text-warning mr-2 text-warning";
    } else {
        icon.className = "fas fa-info-circle text-info mr-2 text-info";
    }

    // Append both
    toastBody.appendChild(icon);
    toastBody.appendChild(text);
    toastHeaderTitle.innerText = title == "" ? type.toUpperCase() : title;

    const toast = new bootstrap.Toast(toastEl, {
        delay: timeShow,
        autohide: true
    });
    toast.show();
}
