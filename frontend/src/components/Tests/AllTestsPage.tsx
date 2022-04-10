import React from 'react';
import { AppShell, Header, Image , SimpleGrid , Space , Grid ,Card , Text} from '@mantine/core';
import CreateTest from './CreateTest';
import { Sidebar } from '../AppShell/SideBar';
import TestCard from './TestCard';

function AllTestsPage() {
    return (
        <AppShell
                header={<Header height={80} p="xs">{
                <div style={{ width: 150, height: 60, display: "fixed" }}>
                    <Image
                    radius="md"
                    src="https://www.zion-net.co.il/wp-content/uploads/2021/06/ZioNET-Logo_new_new-300x127.png"
                    alt="Zionet Logo"
                    />
                </div>
                }</Header>}
                padding="md"
                fixed navbar={<Sidebar/>}
            >
                {

                        <Grid >
                             <Grid.Col md={6} lg={3}>  <Card shadow="sm" p="lg" style={{ width: 340, margin: 'auto', height:307.23 }}>
                             <Text
                                    
                                    align="center"
                                    text-align= "center"
                                   
                                    variant="gradient"
                                    gradient={{ from: 'indigo', to: 'cyan', deg: 45 }}
                                    size="xl"
                                    weight={700}
                                    style={{ fontFamily: 'Greycliff CF, sans-serif' , padding: "90px"}}
                                    >
                                    New Test <br/> +
                                    </Text>  
                                 </Card> </Grid.Col>
                        <Grid.Col md={6} lg={3}> <TestCard/> </Grid.Col>
                        <Grid.Col md={6} lg={3}> <TestCard/> </Grid.Col>
                        <Grid.Col md={6} lg={3}><TestCard/> </Grid.Col>
                        <Grid.Col md={6} lg={3}><TestCard/> </Grid.Col>
                        <Grid.Col md={6} lg={3}><TestCard/> </Grid.Col>
                        </Grid>

                }
            </AppShell>
    );
}

export default AllTestsPage;