package com.example.demo;

import com.example.demo.memo.Memo;
import com.fasterxml.jackson.core.type.TypeReference;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.hamcrest.Matchers;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.mockito.ArgumentCaptor;
import org.mockito.ArgumentMatchers;
import org.mockito.Captor;
import org.mockito.internal.matchers.CapturingMatcher;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.test.web.servlet.MockMvc;
import org.springframework.test.web.servlet.MvcResult;
import org.springframework.test.web.servlet.request.MockMvcRequestBuilders;
import org.springframework.test.web.servlet.result.MockMvcResultHandlers;
import org.springframework.test.web.servlet.result.MockMvcResultMatchers;
import org.springframework.test.web.servlet.setup.MockMvcBuilders;
import org.springframework.util.Assert;
import org.springframework.web.context.WebApplicationContext;

import java.util.List;

import static org.assertj.core.api.Assertions.assertThat;
import static org.hamcrest.Matchers.containsString;
import static org.mockito.ArgumentMatchers.contains;
import static org.springframework.security.test.web.servlet.request.SecurityMockMvcRequestPostProcessors.csrf;
import static org.springframework.security.test.web.servlet.request.SecurityMockMvcRequestPostProcessors.user;
import static org.springframework.security.test.web.servlet.setup.SecurityMockMvcConfigurers.springSecurity;
import static org.springframework.test.util.AssertionErrors.assertTrue;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.*;
import static org.springframework.test.web.servlet.result.MockMvcResultHandlers.log;
import static org.springframework.test.web.servlet.result.MockMvcResultHandlers.print;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.content;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.status;

@SpringBootTest
class DemoApplicationTests {

	@Autowired
	private ObjectMapper mapper;

	@Autowired
	private WebApplicationContext context;

	private MockMvc mockMvc;

	@BeforeEach
	public void setup() {
		mockMvc = MockMvcBuilders.webAppContextSetup(context)
				.apply(springSecurity())
				.build();
	}

	@Test
	void userCanCreateRetrieveUpdateAndDeleteMemos() throws Exception {
		// Create one memo
		byte[] result = mockMvc.perform(post("/memos/").content("user1-note1")
						.with(user("user1"))
						.with(csrf()))
				.andExpect(status().isOk())
				.andReturn().getResponse().getContentAsByteArray();
		// remember newly created memo in order to assert against it
		Memo x = mapper.readValue(result, Memo.class);


		// list all memos
		result = mockMvc.perform(get("/memos/")
						.with(user("user1")))
				.andExpect(status().isOk())
				.andReturn().getResponse().getContentAsByteArray();
		List<Memo> allMemos = mapper.readValue(result, new TypeReference<>() {});
		assertThat(allMemos).contains(x);

		mockMvc.perform(get("/memos/"+x.id+"/perms/")
						.with(user("user1")))
				.andDo(print())
				.andExpect(status().isOk());

		// delete memo that we have created
		mockMvc.perform(delete("/memos/" + x.id)
						.with(user("user1"))
						.with(csrf()))
				.andExpect(status().isOk());


		// list all memos again expect that there is no deleted one anymore
		result = mockMvc.perform(get("/memos/")
						.with(user("user1")))
				.andExpect(status().isOk())
				.andReturn().getResponse().getContentAsByteArray();
		allMemos = mapper.readValue(result, new TypeReference<>() {});
		assertThat(allMemos).doesNotContain(x);

	}

	@Test
	public void usersCanNotSeeEachOtherMemosByDefault() throws Exception {
		byte[] result = mockMvc.perform(post("/memos/").content("user1-memo1")
						.with(user("user1"))
						.with(csrf()))
				.andExpect(status().isOk())
				.andReturn().getResponse().getContentAsByteArray();
		Memo u1m1 = mapper.readValue(result, Memo.class);

		// make sure that we can retrieve object we just created
		mockMvc.perform(get("/memos/" + u1m1.id)
				.with(user("user1")))
				.andExpect(status().isOk());

		// but other users can not access it even if they know id
		mockMvc.perform(get("/memos/"+u1m1.id)
				.with(user("user2")))
				.andExpect(status().isForbidden());

		result = mockMvc.perform(post("/memos/").content("user2-memo1")
						.with(user("user2"))
						.with(csrf()))
				.andExpect(status().isOk())
				.andReturn().getResponse().getContentAsByteArray();
		Memo u2m1 = mapper.readValue(result, Memo.class);

		result = mockMvc.perform(get("/memos/")
						.with(user("user1")))
				.andExpect(status().isOk())
				.andReturn().getResponse().getContentAsByteArray();
		List<Memo> u1Memos = mapper.readValue(result, new TypeReference<>(){});

		assertThat(u1Memos).contains(u1m1);
		assertThat(u1Memos).doesNotContain(u2m1);


		mockMvc.perform(delete("/memos/" + u1m1.id)
						.with(user("user2"))
						.with(csrf()))
				.andExpect(status().isForbidden());

		mockMvc.perform(get("/memos/"+u1m1.id+"/perms/")
				.with(user("user1")))
				.andDo(print())
				.andExpect(status().isOk());

		mockMvc.perform(delete("/memos/" + u1m1.id)
						.with(user("user1"))
						.with(csrf()))
				.andExpect(status().isOk());

		mockMvc.perform(get("/memos/"+u1m1.id+"/perms/")
				.with(user("user1")))
				.andDo(print())
				.andExpect(status().isForbidden());
	}

	@Test
	public void userCanShareMemoWithAnotherUser() throws Exception {
		byte[] result = mockMvc.perform(post("/memos/").content("user1-note")
						.with(user("user1"))
						.with(csrf()))
				.andExpect(status().isOk())
				.andReturn().getResponse().getContentAsByteArray();
		Memo u1m1 = mapper.readValue(result, Memo.class);

		// make sure that we can retrieve object we just created
		mockMvc.perform(get("/memos/" + u1m1.id)
						.with(user("user1")))
				.andExpect(status().isOk());

		// but other users can not access it even if they know id
		mockMvc.perform(get("/memos/"+u1m1.id)
						.with(user("user2")))
				.andExpect(status().isForbidden());

		// but if we add read permission to user2
		mockMvc.perform(post("/memos/"+u1m1.id + "/perms/")
				.content("user2")
				.with(user("user1"))
				.with(csrf()))
				.andExpect(status().isOk());

		// now user2 should be able to read same object
		mockMvc.perform(get("/memos/"+u1m1.id)
				.with(user("user2")))
				.andExpect(status().isOk());
	}
}
