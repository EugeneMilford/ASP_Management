document.addEventListener('DOMContentLoaded', function () {
    var calendarEl = document.getElementById('calendar');
    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'dayGridMonth', // Initial view of the calendar
        headerToolbar: {
            left: 'prev,next today',
            center: 'title',
            right: 'dayGridMonth,timeGridWeek,timeGridDay'
        },
        editable: true, // Enable dragging and resizing events
        selectable: true, // Enable date selection
        select: function (info) {
            // Handle date selection 
            console.log('Selected Date:', info.startStr);
        },
        eventClick: function (info) {
            // Handle event click 
            console.log('Event clicked:', info.event);

            // Confirm deletion
            if (confirm("Do you want to delete this event?")) {
                info.event.remove(); // Remove the event from the calendar
            }
        }
    });

    // Render the calendar
    calendar.render();

    // Handle form submission for adding an entry
    document.getElementById('calendarForm').addEventListener('submit', function (event) {
        event.preventDefault(); // Prevent default form submission

        // Fetch values from form
        let date = document.getElementById('entryDate').value;
        let description = document.getElementById('entryDescription').value;

        // Add event to FullCalendar
        calendar.addEvent({
            title: description,
            start: date
        });

        // Clear form fields
        document.getElementById('entryDate').value = '';
        document.getElementById('entryDescription').value = '';
    });
});