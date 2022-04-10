import React from 'react';
import { AppShell, Header, Image } from '@mantine/core';
import CreateTest from './CreateTest';
import { Sidebar } from '../AppShell/SideBar';

function TestsPage() {
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
          <CreateTest/>
        }
      </AppShell>
    );
}

export default TestsPage;