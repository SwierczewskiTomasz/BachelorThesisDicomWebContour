// source: https://github.com/vikpe/react-webpack-typescript-starter
import * as React from "react";
// import "./../assets/scss/App.scss";
import CornerstoneElement from "./DicomViewer";
import { AppBar, Typography, IconButton, Drawer } from "@material-ui/core";
import FileInputButton from "./FileInput";
import { number } from "prop-types";
import SideMenu from "./SideMenu";
import MenuIcon from "@material-ui/icons/MenuOutlined";
import { connect } from "react-redux";
import { orthancURL } from "../helpers/requestHelper";
import DisplayImage from "./DisplayImage";


export interface AppViewProps {
}

interface AppViewState {
    readonly drawerOpen: boolean;
    readonly currentInstanceId: number;
    // readonly stack: {
    //     imageIds: string[];
    //     currentImageIdIndex: number;
    // };
}


const imageId = "http://localhost:1337/localhost:8042/instances/10806ac1-e3c802f8-3c5c231a-32836ff4-64a7717d/preview";

export default class AppView extends React.Component<AppViewProps, AppViewState> {
    constructor(props: AppViewProps) {
        super(props);
        this.state = {
            drawerOpen: false,
            currentInstanceId: 0
            // stack: {
            //     imageIds: [imageId],
            //     currentImageIdIndex: 0
            // }
        };
    }

    openDrawer() {
        this.setState(prev => ({ drawerOpen: true }));
    }

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
                        <IconButton onClick={() => this.openDrawer()} style={{ marginRight: "1rem" }}>
                            <MenuIcon style={{ color: "fff" }} />
                        </IconButton>
                        DICOM contour
                    </Typography>
                </AppBar>
                <div style={{ width: "100%", textAlign: "center" }}>
                    <Drawer open={this.state.drawerOpen}
                        onClose={() => this.setState({ drawerOpen: false })}>
                        <SideMenu />
                    </Drawer>
                    <DisplayImage />
                    {/* <div style={{ overflow: "auto", height: "100%", float: "left" }}>
                        <CornerstoneElement stack={{ ...this.state.stack }} />
                    </div> */}
                </div>
            </>
        );
    }
}

