(function () {
    var app = angular.module('chatAppHome', []);

    app.controller('MessageCtrl', function ($scope) {
        $scope.messages = [{
            Name: 'Ben Marcus',
            Message: "Hi  : )"
        }, {
            Name: 'Michelle Pepe',
            Message: "What's up?"
        }, {
            Name: 'Ben Marcus',
            Message: "Nothing much, You?"
        }];

        $scope.rosters = [];
        $scope.chatHub = null; 
        $scope.rostersTest = 'Test';
        $scope.chatHub = $.connection.chatHub; 
        $.connection.hub.start();
        $scope.chatHub.server.login('satish', '12345');
        $scope.chatHub.client.onRosterItem = function (rosters) {
            alert(rosters[0].Name);
            $scope.rosters = rosters;
            $scope.$apply();
        }
    });

})();