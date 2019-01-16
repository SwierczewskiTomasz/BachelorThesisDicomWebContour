// source: https://github.com/vikpe/react-webpack-typescript-starter
import * as React from "react";
import { AppBar, Typography } from "@material-ui/core";
import SideMenu from "./SideMenu";
import DisplayImage from "./DisplayImage";
import ContourList from "./ContourList";


export interface AppViewProps {
}

export default class AppView extends React.Component<AppViewProps> {

    render() {
        return (
            <>
                <AppBar
                    position="sticky"
                    color="primary"
                    style={{ display: "inline-block" }}
                >
                    <Typography
                        variant="h5"
                        color="inherit"
                        style={{ margin: "1rem" }}
                    >
                        DICOM contour
                    </Typography>
                </AppBar>
                <div style={{ float: "left", width: "20%" }}>
                    <SideMenu />
                </div>
                <div style={{ float: "left", width: "60%", textAlign: "center" }}>
                    <DisplayImage />
                </div>
                <div style={{ float: "left", width: "20%" }}>
                    <ContourList />
                </div>
            </>
        );
    }
}

