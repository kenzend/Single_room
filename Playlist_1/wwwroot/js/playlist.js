﻿/**
 * Author:    Thang Nguyen
 * Date:      12/11/2020
 * Course:    CS 4540, University of Utah, School of Computing
 * Copyright: CS 4540 and Thang Nguyen - This work may not be copied for use in Academic Coursework.
 *
 * I, Thang Nguyen, certify that I wrote this code from scratch and did
 * not copy it in part or whole from another source.  Any references used
 * in the completion of the assignment are cited in my README file and in
 * the appropriate method header.
 *
 * File Contents
 *
 * Signalr client library
 */

"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/playlisthub").build();

//Disable send button until connection is established
$("#postButton").hide();


/**
 * What to do when a message comes from the Server
 */
connection.on("ReceiveMedia", function (time, user, message, title, url) {
    if (!url)
        $("#mediaList > tbody").append(`<tr id="${count}" onclick="play('${count}');"><td style="text-align: center;">${count}</td><td>${time}</td><td>${user}</td><td>${title}</td><td><audio id="media_${count}" onended="playNext('${count}');" allow="autoplay" class="uninteractable" controls><source src="/room/${message}" /></audio></td><td><button id="remove_${count}" class="Button_Red" onclick="remove('${count}');event.stopPropagation();">Remove</button></td></tr>`);
    else
        $("#mediaList > tbody").append(`<tr id="${count}" onclick="play('${count}');"><td style="text-align: center;">${count}</td><td>${time}</td><td>${user}</td><td>${title}</td><td><video width="300" height="80" id="media_${count}" onended="playNext('${count}');" allow="autoplay" class="uninteractable"><source src="/room/${message}" /></video></td><td><button id="remove_${count}" class="Button_Red" onclick="remove('${count}');event.stopPropagation();">Remove</button></td></tr>`);

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

connection.on("UserJoined", function (user, status, role, joined) {
    var old_offline = document.getElementById(user);
    if (old_offline != null)
        document.getElementById(user).parentNode.removeChild(old_offline);
    $("#userList > tbody").prepend(`<tr id="${user}" class="online"><td>${user}</td><td>${joined}</td><td>${role}</td><td><button id="promote_${user}" onclick="promote('${user}');">Promote</button><button id="block_${user}" onclick="block('${user}');">Block</button><button id="remove_media_of_${user}" onclick="remove_media_of('${user}');">Remove Media(s)</button><button id="demote_${user}" onclick="demote('${user}');">Demote</button></td><td>${status}</td></tr>`);
    
});

connection.on("UserOff", function (user, status, role, joined) {
    var old_online = document.getElementById(user);
    if (old_online != null)
        document.getElementById(user).parentNode.removeChild(old_online);
    $("#userList > tbody").append(`<tr id="${user}" class="offline"><td>${user}</td><td>${joined}</td><td>${role}</td><td><button id="promote_${user}" onclick="promote('${user}');">Promote</button><button id="block_${user}" onclick="block('${user}');">Block</button><button id="remove_media_of_${user}" onclick="remove_media_of('${user}');">Remove Media(s)</button><button id="demote_${user}" onclick="demote('${user}');">Demote</button></td><td>${status}</td></tr>`);
});

connection.on("RemoveUser", function (user) {
    var to_be_removed = document.getElementById(user);
    document.getElementById(user).parentNode.removeChild(to_be_removed);
});

connection.on("Leave", function (user) {
    connection.stop();
});

connection.on("Promoted", function (user, new_role) {
    document.getElementById(user).getElementsByTagName("td").item(2).innerText = new_role;
});

connection.on("Demoted", function (user, new_role) {
    document.getElementById(user).getElementsByTagName("td").item(2).innerText = new_role;
});

connection.on("Blocked", function (user, status, role, joined) {
    var old = document.getElementById(user);
    if (old != null)
        document.getElementById(user).parentNode.removeChild(old);
    $("#userList > tbody").append(`<tr id="${user}" class="blocked"><td>${user}</td><td>${joined}</td><td>${role}</td><td><button id="unblock_${user}" onclick="unblock('${user}');">Unblock</button><button id="remove_media_of_${user}" onclick="remove_media_of('${user}');">Remove Media(s)</button></td><td>${status}</td></tr>`);
});


/**
 *  Main initiation of web socket connection (via signalR)
 */
connection
    .start()
    .then(
        function () {
            $("#postButton").show();
            connection.invoke("JoinRoom", user)
                .catch(function (err) {
                    return console.error(err.toString());
                });
        })
    .catch(
        function (err) {
            return console.error(err.toString());
        });


$("#postButton").click(
    function (event) {
        var userStatus = document.getElementById(user).getElementsByTagName("td").item(4).innerText;
        if (userStatus == "blocked") {
            document.getElementById('linkInput').value = null;
            document.getElementById('fileInput').value = null;
            return;
        }
            
        var message = $("#linkInput").val();
        var input = document.getElementById('fileInput');
        var files = input.files;
        var formData = new FormData();

        if (!message && files.length == 0) // both inputs are empty
            return;
        else if (message && files.length == 0) // only link
        {
            connection.invoke("ShareLink", user, message)
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
                    for (j = 0; j < result.length; j+=2) {
                        connection.invoke("ShareMedia", user, result[j], result[j+1])
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
                    for (j = 0; j < result.length; j+=2) {
                        connection.invoke("ShareMedia", user, result[j], result[j+1])
                            .catch(function (err) {
                                return console.error(err.toString());
                            });
                    }
                    connection.invoke("ShareLink", user, message)
                        .catch(function (err) {
                            return console.error(err.toString());
                        });
                },
                error: function () {
                    connection.invoke("ShareLink", user, message)
                        .catch(function (err) {
                            return console.error(err.toString());
                        });
                }
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
        var userRole = document.getElementById(user).getElementsByTagName("td").item(2).innerText;
        if (userRole == "Anon")
            return;
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
        connection.invoke("Pause", user, currentlyPlaying)
            .catch(function (err) {
                return console.error(err.toString());
            });
    });

$('#next').click(
    function (event) {
        var userRole = document.getElementById(user).getElementsByTagName("td").item(2).innerText;
        if (userRole == "Anon")
            return;
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
    var userRole = document.getElementById(user).getElementsByTagName("td").item(2).innerText;
    if (userRole == "Anon")
        return;
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

function promote(name) {
    connection.invoke("Promote", name)
        .catch(function (err) {
            return console.error(err.toString());
        });
}

function demote(name) {
    connection.invoke("Demote", name)
        .catch(function (err) {
            return console.error(err.toString());
        });
}

function block(name) {
    connection.invoke("Block", name)
        .catch(function (err) {
            return console.error(err.toString());
        });
}

function remove_media_of(name) {
    connection.invoke("RemoveMediaOf", name)
        .catch(function (err) {
            return console.error(err.toString());
        });
}

function unblock(name) {
    connection.invoke("Unblock", name)
        .catch (function (err) {
            return console.error(err.toString());
        });
}