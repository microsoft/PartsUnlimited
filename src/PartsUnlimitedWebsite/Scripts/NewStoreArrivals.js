$(function () {
    $.connection.hub.logging = true;
    var announcementsHub = $.connection.Announcement;

    announcementsHub.client.announcement = function (item) {
        var newArrival = $('#new-arrival');

        newArrival.attr("href", item.Url);
        newArrival.text(" New Arrival : " + item.Title);

        $("#new-arrival").show();
    };

    $.connection.hub.start().done(function () {
        console.log('hub connection open');
    });
});