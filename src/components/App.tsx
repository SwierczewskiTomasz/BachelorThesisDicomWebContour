// source: https://github.com/vikpe/react-webpack-typescript-starter
import * as React from "react";
import "./../assets/scss/App.scss";
import CornerstoneElement from "./DicomViewer";

const reactLogo = require("./../assets/img/react_logo.svg");

export interface AppProps {
}

const imageId =
    "https://rawgit.com/cornerstonejs/cornerstoneWebImageLoader/master/examples/Renal_Cell_Carcinoma.jpg";

const stack = {
    imageIds: [imageId],
    currentImageIdIndex: 0
};

export default class App extends React.Component<AppProps, undefined> {
    render() {
        return (
            <div className="app">
                <h1>Hello World!</h1>
                <p>Foo to the barz</p>
                <img src={reactLogo} height="480"/>
                <CornerstoneElement stack={{ ...stack }} />
            </div>
        );
    }
}
