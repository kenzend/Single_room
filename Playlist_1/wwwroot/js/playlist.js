"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/playlisthub").build();

//Disable send button until connection is established
$("#postButton").hide();


/**
 * What to do when a message comes from the Server
 */
connection.on("ReceiveMedia", function (time, user, message) {
    if (!message.includes("http")) {
        $("#mediaList > tbody").append(`<tr id="${count}"><td style="text-align: center;">${count}</td><td>${time}</td><td>${user}</td><td><audio id="media_${count}" onended="playNext('${count}');" allow="autoplay" controls><source src="/room/${message}" /></audio></td><td><button id="play_${count}" onclick="play('${count}');">Play</button><button id="pause_${count}" onclick="document.getElementById('media_${count}').pause()">Pause</button><button id="remove_${count}" onclick="remove('${count}');">Remove</button></td></tr>`);
    }
    else
    {
        $("#mediaList > tbody").append(`<tr id="${count}"><td>${time}</td><td>${user}</td><td>${message}</td></tr>`);
    }
    
    $(`#${count}`).hide();
    $(`#${count}`).fadeIn("slow");

    //setTimeout(function () { $(`#id${count}`).toggleClass("highlight", 1000) }, 5000);
    count++;
});

connection.on("RemovedMedia", function (index) {
    var to_be_removed = document.getElementById(index);
    document.getElementById(index).parentNode.removeChild(to_be_removed);
    var currPlay = $('#currentlyPlayingIndex').html();
    if (index == currPlay)
        document.getElementById("currentlyPlayingIndex").textContent = "none";
});

connection.on("PlayMedia", function (index) {
    document.getElementById("media_" + index).play();
    document.getElementById("currentlyPlayingIndex").textContent = index;
});

connection.on("ResetCurrentTime", function (index) {
    document.getElementById("media_" + index).currentTime = 0;
});

connection.on("PauseMedia", function (index) {
    document.getElementById('media_' + index).pause();
});

/**
 *  Main initiation of web socket connection (via signalR)
 */
connection
    .start()
    .then(
        function () {
            $("#postButton").show();
        })
    .catch(
        function (err) {
            return console.error(err.toString());
        });

$("#postButton").click(
    function (event) {
        var message = $("#linkInput").val();
        var input = document.getElementById('fileInput');
        var files = input.files;
        var formData = new FormData();

        if (!message && files.length == 0) // both inputs are empty
            return;
        else if (message && files.length == 0) // only link
        {
            connection.invoke("ShareMedia", user, message)
                .catch(function (err) {
                    return console.error(err.toString());
                });
        }
        else if (!message && files.length > 0) // only file(s)
        {
            for (var i = 0; i != files.length; i++) {
                formData.append("files", files[i]);
            }
            $.ajax({
                url: '/Room/UploadFile',
                type: 'POST',
                headers: {
                    RequestVerificationToken:
                        $('input:hidden[name="__RequestVerificationToken"]').val()
                },
                data: formData,
                processData: false,
                contentType: false,
                success: function (result) {
                    var j;
                    for (j = 0; j < result.length; j++) {
                        connection.invoke("ShareMedia", user, result[j])
                            .catch(function (err) {
                                return console.error(err.toString());
                            });
                    }
                }
            });
        }
        else if (message && files.length > 0) // upload both
        {
            for (var i = 0; i != files.length; i++) {
                formData.append("files", files[i]);
            }
            $.ajax({
                url: '/Room/UploadFile',
                type: 'POST',
                headers: {
                    RequestVerificationToken:
                        $('input:hidden[name="__RequestVerificationToken"]').val()
                },
                data: formData,
                processData: false,
                contentType: false,
                success: function (result) {
                    var j;
                    for (j = 0; j < result.length; j++) {
                        connection.invoke("ShareMedia", user, result[j])
                            .catch(function (err) {
                                return console.error(err.toString());
                            });
                    }
                }
            });
            connection.invoke("ShareMedia", user, message)
                .catch(function (err) {
                    return console.error(err.toString());
                });
        }
        document.getElementById('linkInput').value = null;
        document.getElementById('fileInput').value = null;
    });

function remove(index) {
    connection.invoke("RemoveMedia", user, index)
        .catch(function (err) {
            return console.error(err.toString());
        });
}

$('#play_from_beginning').click(
    function (event) {
        var currPlay = $('#currentlyPlayingIndex').html();
        document.getElementById("pause").click();
        connection.invoke("ResetCurrentTime", currPlay)
            .catch(function (err) {
                return console.error(err.toString());
            });
        var currentIndex = '-1';
        connection.invoke("PlayNext", currentIndex)
            .catch(function (err) {
                return console.error(err.toString());
            });
    });

$('#play').click(
    function (event) {
        var currentlyPlaying = $('#currentlyPlayingIndex').html();
        connection.invoke("Play", user, currentlyPlaying)
            .catch(function (err) {
                return console.error(err.toString());
            });
    });

$('#pause').click(
    function (event) {
        var currentlyPlaying = $('#currentlyPlayingIndex').html();
        connection.invoke("Pause", currentlyPlaying)
            .catch(function (err) {
                return console.error(err.toString());
            });
    });

$('#next').click(
    function (event) {
        var currentlyPlaying = $('#currentlyPlayingIndex').html();
        playNext(currentlyPlaying);
    });

function playNext(currentIndex) {
    //var currentIndex_num = parseInt(currentIndex);
    //connection.invoke("CurrentTime", currentIndex)
    //    .catch(function (err) {
    //        return console.error(err.toString());
    //    });
    //if (currentIndex_num == (count-1))
    //{
    //    document.getElementById("play").click();
    //    return;
    //}
    //++currentIndex_num;
    //document.getElementById("play_" + currentIndex_num.toString()).click();
    document.getElementById("pause").click();
    connection.invoke("ResetCurrentTime", currentIndex)
        .catch(function (err) {
            return console.error(err.toString());
        });
    connection.invoke("PlayNext", currentIndex)
        .catch(function (err) {
            return console.error(err.toString());
        });
}

function play(index) {
    var currPlay = $('#currentlyPlayingIndex').html();
    document.getElementById("pause").click();
    connection.invoke("ResetCurrentTime", currPlay)
        .catch(function (err) {
            return console.error(err.toString());
        });
    connection.invoke("Play", user, index)
        .catch(function (err) {
            return console.error(err.toString());
        });
}

function pause(index) {
    connection.invoke("Pause", user, index)
        .catch(function (err) {
            return console.error(err.toString());
        });
}