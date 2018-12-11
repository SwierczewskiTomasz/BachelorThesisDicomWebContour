import * as React from "react";
import { Drawer, List, ListItem } from "@material-ui/core";
import { connect } from "react-redux";
import { Dispatch } from "redux";
import { startTask, endTask } from "../helpers/asyncActions";
import { fetchSeries } from "../containers/series/reducers";

export interface SideMenuProps {
    readonly seriesIds: string[];
    readonly fetchSeriesIds: () => void;
}

class SideMenuView extends React.Component<SideMenuProps> {

    componentDidMount() {
        this.props.fetchSeriesIds();
    }

    render() {

        return (
            <List>
                {this.props.seriesIds.map( id => <ListItem key={id}>{id}</ListItem>)}
            </List>
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
            dispatch(fetchSeries());
        }
    })
)(SideMenuView);
