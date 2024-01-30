// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function () {
    var calendarEl = document.getElementById('calendar');
    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth',
        selectable: true,
        dateClick: function (info) {
            var entry = prompt('Enter your entry for ' + info.dateStr);
            if (entry) {
                // Here you can add your logic to save the entry
                // For demo, let's just log it to console
                console.log('Entry for ' + info.dateStr + ': ' + entry);
            }
        }
    });
    calendar.render();
});
