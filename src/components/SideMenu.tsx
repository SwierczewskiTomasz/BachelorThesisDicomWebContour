import * as React from "react";
import { Drawer } from "@material-ui/core";
import { connect } from "react-redux";
import { Dispatch } from "redux";

export interface SideMenuProps {
    readonly seriesIds: string[];
    // readonly fetchSeriesIds: () => void;
}

class SideMenuView extends React.Component<SideMenuProps> {

    render() {

        return (
            <Drawer>
                <div>TEST</div>
            </Drawer>
        );
    }
}

export default connect(
    (state: AppState) => {
        return {
            seriesIds: state.seriesIds
        };
    },
    (dispatch: Dispatch<any>) => ({
        fetchSeriesIds: () => {
            // dispatch();
        }
    })
)(SideMenuView);
