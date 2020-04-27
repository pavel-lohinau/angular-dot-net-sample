"use strict";
var __spreadArrays = (this && this.__spreadArrays) || function () {
    for (var s = 0, i = 0, il = arguments.length; i < il; i++) s += arguments[i].length;
    for (var r = Array(s), k = 0, i = 0; i < il; i++)
        for (var a = arguments[i], j = 0, jl = a.length; j < jl; j++, k++)
            r[k] = a[j];
    return r;
};
Object.defineProperty(exports, "__esModule", { value: true });
var router_1 = require("@angular/router");
var fetch_data_routes_1 = require("./fetch-data/fetch-data.routes");
var counter_routes_1 = require("./counter/counter.routes");
var home_routes_1 = require("./home/home.routes");
var login_routes_1 = require("./login/login.routes");
var register_routes_1 = require("./register/register.routes");
var handleWrongUrls = [
    { path: '**', redirectTo: '' }
];
exports.AppRoutes = __spreadArrays(fetch_data_routes_1.FetchDataRoutes, counter_routes_1.CounterRoutes, home_routes_1.HomeRoutes, login_routes_1.LoginRoutes, register_routes_1.RegisterRoutes, handleWrongUrls);
exports.Routing = router_1.RouterModule.forRoot(exports.AppRoutes);
//# sourceMappingURL=app.routes.js.map