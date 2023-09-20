/*-
 * Copyright 2018-2020 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
package de.ingogriebsch.sample.spring.web.restcontroller;

import static org.springframework.http.HttpStatus.CREATED;
import static org.springframework.http.ResponseEntity.badRequest;
import static org.springframework.http.ResponseEntity.notFound;
import static org.springframework.http.ResponseEntity.ok;
import static org.springframework.http.ResponseEntity.status;

import java.util.Set;

import lombok.NonNull;
import lombok.RequiredArgsConstructor;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RequestMapping(PersonController.BASE_PATH)
@RequiredArgsConstructor
@RestController
class PersonController {

    static final String BASE_PATH = "/persons";
    static final String PATH_FIND_ONE = "/{personId}";
    static final String PATH_DELETE = "/{personId}";

    @NonNull
    private final PersonService personService;

    @GetMapping
    Set<Person> findAll() {
        return personService.findAll();
    }

    @GetMapping(PATH_FIND_ONE)
    ResponseEntity<Person> findOne(@PathVariable String personId) {
        return personService.findOne(personId).map(ResponseEntity::ok).orElse(notFound().build());
    }

    @PostMapping
    ResponseEntity<Void> insert(@RequestBody Person person) {
        return personService.insert(person) ? status(CREATED).build() : badRequest().build();
    }

    @DeleteMapping(PATH_DELETE)
    ResponseEntity<Void> delete(@PathVariable String personId) {
        return personService.delete(personId) ? ok().build() : notFound().build();
    }
}
