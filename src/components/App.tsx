// source: https://github.com/vikpe/react-webpack-typescript-starter
import * as React from "react";
// import "./../assets/scss/App.scss";
import CornerstoneElement from "./DicomViewer";
import { AppBar, Typography } from "@material-ui/core";
import FileInputButton from "./FileInput";
import { number } from "prop-types";
import SideMenu from "./SideMenu";


export interface AppProps {
}

interface AppState {
    readonly stack: {
        imageIds: string[];
        currentImageIdIndex: number;
    };
}


const imageId = "http://localhost:1337/localhost:8042/instances/10806ac1-e3c802f8-3c5c231a-32836ff4-64a7717d/preview";

export default class App extends React.Component<AppProps, AppState> {
    constructor(props: AppProps) {
        super(props);
        this.state = {
            stack: {
                imageIds: [imageId],
                currentImageIdIndex: 0
            }
        };
    }

    render() {
        return (
            <>
                <AppBar
                    position="sticky"
                    color="primary"
                >
                    <Typography
                        variant="h5"
                        color="inherit"
                        style={{ margin: "1rem" }}
                    >
                        DICOM contour
                    </Typography>
                </AppBar>
                <SideMenu />
                <div style={{ overflow: "auto", height: "100%" }}>
                    <CornerstoneElement stack={{ ...this.state.stack }} />
                </div>
            </>
        );
    }
}
