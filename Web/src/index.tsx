// source: https://github.com/vikpe/react-webpack-typescript-starter
import * as React from "react";
import { render } from "react-dom";
import AppView from "./components/App";
import { Provider } from "react-redux";
import store from "./Store";

const rootEl = document.getElementById("root");

render(
    <Provider store={store}>
        <AppView />
    </Provider>,
    rootEl
);

// Hot Module Replacement API
declare let module: { hot: any };

if (module.hot) {
    module.hot.accept("./components/App", () => {
        const NewApp = require("./components/App").default;

        render(
            <Provider store={store}>
                <NewApp />
            </Provider>,
            rootEl
        );
    });
}
