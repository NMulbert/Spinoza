import React from 'react';
import { useState } from 'react';
import { RadioGroup, Radio, Textarea, MultiSelect, TextInput, SimpleGrid, Group, Button, Text, Badge, ActionIcon, Space, Divider } from '@mantine/core';
import {  Hash } from 'tabler-icons-react';


function NewQuestion() {

    const [dataHash, setHashData] = useState(['React', 'C#', 'JavaScript', 'Python']);

    return (
        <div>
            <SimpleGrid cols={1}>
                <div>
                    <TextInput label="Question Name:" style={{ width: "50%", textAlign: 'left' }} placeholder="Question Name" radius="xs"/>
                    <MultiSelect
                            data={dataHash}
                            label="Tags:"
                            placeholder="Select tags"
                            icon={<Hash />}
                            radius="xs"
                            style={{ width: "50%"}}
                            searchable
                            creatable
                            getCreateLabel={(query) => `+ Create ${query}`}
                            onCreate={(query) => setHashData((current) => [...current, query])}
                        />
                        <Textarea
                            placeholder="Write question here..."
                            label="Description:"
                            radius="xs"
                            autosize
                            minRows={4}
                        />
                </div>
                <div>
                    <Text weight="500" size="sm">Answer Type:</Text>
                    <Space h="xs"/>
                    <Group spacing="xs">
                    <Button radius="md" size="xl" variant="outline" color="pink">
                    Text</Button>
                    <Button radius="md" size="xl" variant="outline" color="violet">
                    Multiple Choice</Button>
                    </Group>
                    <Space h="xs"/>
                    <RadioGroup
                    label="Select level:"
                    required
                    >
                    <Radio value="level1" label="1" />
                    <Radio value="level2" label="2" />
                    <Radio value="level3" label="3" />
                    <Radio value="level4" label="4" />
                    <Radio value="level5" label="5" />
                    </RadioGroup>
                    <Space h="xs"/>
                    <Divider size="xs" variant="dotted" />
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
        </div>
    );
}

export default NewQuestion;