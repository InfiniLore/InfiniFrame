(function () {
    window.__scriptSrcSmokeLoaded = true;
    window.__scriptSrcSmokeRunCount = (window.__scriptSrcSmokeRunCount ?? 0) + 1;
    window.scriptSrcSmokeEcho = function (value) {
        return "script-src-smoke:" + value;
    };
})();
