import * as React from "react";
import { Drawer, List, ListItem, ListItemIcon, Divider, ListItemSecondaryAction, Icon } from "@material-ui/core";
import { connect } from "react-redux";
import { Dispatch } from "redux";
import { fetchSeries, Serie } from "../containers/series/reducers";
import { fetchPatients, Patient } from "../containers/patients/reducers";
import { fetchStudies, Study } from "../containers/studies/reducers";
import { fetchInstances, setCurrentInstanceInd } from "../containers/instances/reducers";
import ArrowBackIcon from "@material-ui/icons/ArrowBackOutlined";
import PersonIcon from "@material-ui/icons/PersonOutlined";
import CollectionIcon from "@material-ui/icons/CollectionsOutlined";
import PhotoIcon from "@material-ui/icons/PhotoOutlined";
import { Contour, setCurrentContur } from "../containers/contours/reducers";

export interface ContourListProps {
    readonly contours: Contour[];
    readonly setCurrentContur: (guid: string) => void;
}

interface ContourListState {
}


class ContourListView extends React.Component<ContourListProps, ContourListState> {
    constructor(props) {
        super(props);
        this.state = {
        };
    }
    render() {

        return (
            <List>
                <ListItem                >
                    {"Available Contours"}
                </ListItem>
                <Divider />
                {this.props.contours
                    .map(c => <ListItem
                        button
                        key={c.guid}
                        onClick={() => this.props.setCurrentContur(c.guid)}
                    >
                        {c.tag}
                        <ListItemSecondaryAction>
                            <ListItemIcon>
                                <PersonIcon />
                            </ListItemIcon>
                        </ListItemSecondaryAction>
                    </ListItem>)}
            </List >
        );
    }
}

export default connect(
    (state: AppState) => {
        return {
            contours: state.contours,
        };
    },
    (dispatch: Dispatch<any>) => ({
        setCurrentContur: (guid: string) => {
            dispatch(setCurrentContur(guid));
        }
    })
)(ContourListView);
