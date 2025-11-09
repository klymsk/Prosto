document.addEventListener("DOMContentLoaded", function () {
    const tbody = document.querySelector("#usersV2 tbody");

    fetch("/api/v2/UserProfile")
        .then(response => { return response.json(); })
        .then(data => {
            tbody.innerHTML = "";

            data.forEach(u => {
                const row = document.createElement("tr");
                row.innerHTML = `
                    <td>${u.userId}</td>
                    <td>${u.fullName}</td>
                    <td>${u.email}</td>
                    <td>${u.phoneNumber}</td>
                    <td>${u.ordersCount}</td>
                `;
                tbody.appendChild(row); 
            });
        })
        .catch(error => { console.error("Error:", error); });
});