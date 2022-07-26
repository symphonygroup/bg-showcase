package com.example.demo.memo;

import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface Memos extends JpaRepository<Memo, Long> {
    List<Memo> findAllByOwner(String owner) throws Exception;
}
