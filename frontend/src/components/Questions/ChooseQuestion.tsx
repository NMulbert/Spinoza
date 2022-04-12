import { useState, useEffect } from 'react';
import { Search, Hash, ArrowRight } from 'tabler-icons-react';
import { MultiSelect, TextInput, SimpleGrid, Group, Button, Text, ScrollArea, ActionIcon, Divider, Loader, Center } from '@mantine/core';
import ExistingQuestion from './ExistingQuestion';


function ChooseQuestion() {

    const [dataHash, setHashData] = useState(['React', 'C#', 'JavaScript', 'Python']);
    const [questions, setQuestions] = useState([]);

    useEffect(() => {
        let url = "./QuestionObject.json";
        fetch(url)
          .then((res) => res.json())
          .then((result) => {
            setQuestions(result);
          });
      }, []);


    return (
        <div>
            <SimpleGrid cols={1}>
                <div>
                    <Group position="center">
                    <TextInput 
                        icon={<Search />}
                        style={{ width: "90%"}}
                        placeholder="Search"
                        radius="lg"
                        rightSection={
                            <ActionIcon size={32} radius="xl" variant="filled" color="blue">
                                <ArrowRight/>
                            </ActionIcon>
                          }
                        />
                    </Group>
                </div>

                <div>
                <Group position="center" spacing="xs">

                    <MultiSelect
                        data={dataHash}
                        placeholder="Select tags"
                        icon={<Hash />}
                        radius="lg"
                        style={{ width: "70%"}}
                        searchable
                        creatable
                        getCreateLabel={(query) => `+ Create ${query}`}
                        onCreate={(query) => setHashData((current) => [...current, query])}
                    />
                </Group>
                </div>
                <div>
                    <ScrollArea style={{ height: 450, width: "100%" }}>
                        {
                            <SimpleGrid style={{textAlign: "center" }} cols={3}>
                                {questions.length !== 0 ? (
                                    questions.map((i: any) => {
                                    return (
                                        <ExistingQuestion key={i.Id}
                                            Id={i.Id}
                                            Title={i.Title}
                                            Description={i.Description}
                                            Author={i.Author}
                                            Tags={i.Tags}
                                            Status={i.Status}
                                            Version={i.Version}
                                        />
                                    );
                                    })
                                ) : (
                                    <Loader/>
                                )}
                            </SimpleGrid>
                        }
                    </ScrollArea>
                    <Divider size="xs" variant="dotted" />
                </div>
                <div>
                    <Group position="center" spacing="xs">
                    <Text size="md">Selected:</Text>
                    <Text weight="bold" size="md">null</Text>
                    <Button radius="lg">Done</Button>
                    </Group>
                </div>
            </SimpleGrid>
        </div>
    );
}

export default ChooseQuestion;  