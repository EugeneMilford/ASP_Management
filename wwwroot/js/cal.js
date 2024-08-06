$(document).ready(function () {
    $('#calendar').fullCalendar({
        editable: true,
        selectable: true,
        events: '/Calendar/GetEvents',
        select: function (start, end) {
            $('#entryDate').val(moment(start).format('YYYY-MM-DD'));
            $('#calendarForm').show();
        },
        eventClick: function (event) {
            if (confirm("Do you really want to delete this event?")) {
                $.ajax({
                    url: '/Calendar/DeleteEvent',
                    type: 'POST',
                    data: { id: event.id },
                    success: function () {
                        $('#calendar').fullCalendar('removeEvents', event.id);
                        alert("Event Deleted");
                    }
                });
            }
        }
    });

    $('#calendarForm').on('submit', function (e) {
        e.preventDefault();
        var date = $('#entryDate').val();
        var description = $('#entryDescription').val();

        $.ajax({
            url: '/Calendar/AddEvent',
            type: 'POST',
            data: { date: date, description: description },
            success: function (response) {
                $('#calendar').fullCalendar('renderEvent', {
                    id: response.id,
                    title: description,
                    start: date,
                    allDay: true
                });
                alert("Event Added");
                $('#calendarForm').hide();
            }
        });
    });
});