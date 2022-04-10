import React from 'react';
import { useState } from 'react';
import { TextInput, Group, Button, SimpleGrid, Modal } from '@mantine/core';
import ChooseQuestion from '../Questions/ChooseQuestion';

function CreateTest() {
    const [openedNQ, setOpenedNQ] = useState(false);
    const [openedEQ, setOpenedEQ] = useState(false);

    return (
        <>
        <Modal
        opened={openedNQ}
        onClose={() => setOpenedNQ(false)}
        title="New Question"
        size="75%"
        >
        {
            "<NewQuestion/>"
        }
      </Modal>
      <Modal
        opened={openedEQ}
        onClose={() => setOpenedEQ(false)}
        title="Questions Catalog"
        size="75%"
        >
        {
            <ChooseQuestion/>
        }
      </Modal>

        <SimpleGrid cols={1} spacing="xs">
            <h1>New Test</h1>
            <div>
            <TextInput label="Test Name:" style={{ width: "40%", textAlign: 'left' }} placeholder="Test Name" radius="xs"/>
            </div>
            <div>
            <TextInput label="Subject:" style={{ width: "40%", textAlign: 'left' }} placeholder="Subject" radius="xs"/>
            </div>
            <div>
            <TextInput label="Tags:" style={{ width: "40%", textAlign: 'left' }} placeholder="#Tags" radius="xs"/>
            </div>
            <div>
                <Group spacing="sm">
                    <Button variant="outline" radius="lg" onClick={() => setOpenedNQ(true)}>
                    ADD NEW QUESTION</Button>
                    <Button variant="outline" radius="lg" onClick={() => setOpenedEQ(true)} color="dark" >CHOOSE FROM CATALOG</Button>
                </Group>
            </div>
            <div>
                <Group spacing="sm">
                    <Button radius="lg" variant="gradient" gradient={{ from: '#838685', to: '#cfd0d0' }}>
                    Save as Draft</Button>
                    <Button radius="lg" variant="gradient" gradient={{ from: '#217ad2', to: '#4fbaee' }}>
                    Publish</Button>
                </Group>
            </div>
        </SimpleGrid>
        </>
    );
}

export default CreateTest;  