(function () {
    var app = angular.module('chatApp', []);

    app.controller('loginController', function ($scope, $window) {
        // scope variables
        $scope.username = ''; 
        $scope.password = ''; 
        $scope.loginMsg = ''; 
        $scope.loginValidated = false; 

        $('#chatDiv').hide();
        $scope.chatHub = null; // holds the reference to hub
        $scope.chatHub = $.connection.chatHub; // initializes hub
        $.connection.hub.start(); // starts hub
        $scope.chatMsg = '';
        $scope.selectedBare = '';
        $scope.messages = [{
            Name: 'Ravi',
            Message: "Hi  : )"
        }, {
            Name: 'Satish',
            Message: "What's up?"
        }, {
            Name: 'Ravi',
            Message: "Nothing much, You?"
        }];
        $scope.rosters = [{
            uniqueId : "124",
            Bare : "test@123",
            Name : "test"}];

        $scope.chatHub.client.login = function (loginMsg) {
            $scope.loginMsg = loginMsg;
        }

        $scope.rosterSelected = function (rosterBare) {
            $scope.selectedBare = rosterBare;
            $scope.chatHub.server.retrieveArchivedMessages(rosterBare);
        }

        $scope.sendMsg = function () {
            $scope.chatHub.server.sendMessage($scope.selectedBare, $scope.chatMsg);
        }
        
        $scope.login = function () {
            $scope.chatHub.server.login($scope.username, $scope.password);

        }
        $scope.chatHub.client.loggedIn = function (status) {
            //$window.location.href = '/Home.html';
            if (status===true) {
                $('#loginDiv').hide();
                $('#chatDiv').show();
                loginValidated = true;
            }
        }

        $scope.chatHub.client.onRosterItemReceived = function (rosters) {
            $scope.rosters = rosters;
            $scope.$apply();
        }
        $scope.chatHub.client.onMessage = function (from, body) {
            $scope.messages.push({
                Name: from,
                Message: body
            });

            $scope.$apply();
        }

        
    })
}());